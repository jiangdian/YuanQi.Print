using Module.BiaoQian.ViewModels;
using Module.HeGe.ViewModels;
using Module.Home.Enum;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Threading;
using System.Threading.Tasks;
using YuanQiTool;
using YuanQiTool.PLC;
using YuanQiTool.Scanner.Honeywell;
using YuanQiUI.Event;

namespace Module.Home.Bll
{

    public class HomeBll
    {
        #region 属性
        private readonly IContainerExtension container;
        private readonly IEventAggregator eventAggregator;
        private IPLCHelper client;
        private HoneywellTCPScanner scanner;
        public CancellationTokenSource cancellation;
        private HeGeViewModel heGeViewModel;
        private BiaoQianViewModel biaoQianViewModel;
        private readonly object PLCLock = new object { };
        #endregion
        #region 方法
        public bool Init()
        {
            //连接PLC
            if (!InitPLC()) return false;
            //连接扫码枪
            if (!InitScanner()) return false;
            //连接合格证打印机
            if (SysCfg.HeGe && !heGeViewModel.ConnectPrinter()) return false;
            //连接二次标签打印机
            if (SysCfg.BiaoQian && !biaoQianViewModel.ConnectPrinter()) return false;
            return true;
        }
        public bool InitPLC()
        {
            this.eventAggregator.GetEvent<MessageEvent>().Publish("连接PLC...");
            if (!client.PLCConnect(SysCfg.PLC_IP, SysCfg.PLC_PORT))
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("连接PLC失败!"); 
                return false;
            }
            this.eventAggregator.GetEvent<MessageEvent>().Publish("连接PLC成功!"); 
            return true;
        }
        public bool InitScanner()
        {
            this.eventAggregator.GetEvent<MessageEvent>().Publish("连接扫码枪...");
            scanner.Connect(SysCfg.SCAN_IP, SysCfg.SCAN_PORT);
            Thread.Sleep(500);
            if (!scanner.IsConnect())
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("连接扫码枪失败!");
                return false;
            }
            this.eventAggregator.GetEvent<MessageEvent>().Publish("连接扫码枪成功!");
            return true;
        }
        public void ListenPLC()
        {
            cancellation?.Cancel();
            cancellation = new CancellationTokenSource();
            int i=0;
            Task.Run(async() => {
                while (!cancellation.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(500);
                        lock (PLCLock)
                        {
                            var reData = client.Read<ushort>(SysCfg.PLCaddr); //读取节点数据
                            if (reData != (ushort)PLCEnum.Done) continue;//电表未到位
                        }
                        if (SysCfg.BiaoQian) await ReadBarCodeAsync();
                        SendData();
                    }
                    catch (Exception)
                    {
                        this.eventAggregator.GetEvent<MessageEvent>().Publish("监听PLC状态失败！");
                    }
                }
            },cancellation.Token);
        }
        private async Task ReadBarCodeAsync()
        {
            this.eventAggregator.GetEvent<MessageEvent>().Publish("电表到位,触发扫码!");
            if (scanner.IsConnect())
            {
                scanner.BarCodeNo = string.Empty;
                scanner.ReadBarcode();
                await Task.Delay(500);
                if (string.IsNullOrEmpty(scanner.BarCodeNo))
                {
                    await Task.Delay(1000);
                }
                scanner.StopRead();
            }
            else
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("扫码枪连接失败,正在重新连接!");
                scanner.Connect(SysCfg.SCAN_IP, SysCfg.SCAN_PORT);
            }
        }
        private void SendData()
        {
            if (SysCfg.BiaoQian)//二次标签
            {
                string barcode = scanner.BarCodeNo.Trim('\r', '\n');
                this.eventAggregator.GetEvent<BarCodeEvent>().Publish(barcode);
                if (string.IsNullOrEmpty(barcode))
                {
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("扫码为空!");
                    WritePLC(SysCfg.PLCaddr, (ushort)PLCEnum.Fail);
                    return;
                }
                if (!biaoQianViewModel.CreatZPL(scanner.BarCodeNo)) return;
                if (!biaoQianViewModel.SendPrint()) return;
            }
            if (SysCfg.HeGe)//合格证
            {
                if (!heGeViewModel.CreatZPL(string.Empty)) return;
                if (!heGeViewModel.SendPrint())
                { 
                    return; 
                }
                else
                {
                    heGeViewModel.BarcodeCfg.PrintBody =(Convert.ToInt32(SysCfg.HTMBody) + 1).ToString();
                }
            }
           
            WritePLC(SysCfg.PLCaddr, (ushort)PLCEnum.Success);
        }
        private void WritePLC(string adr,ushort value)
        {
            lock (PLCLock)
            {
                client.Write(adr, value);
            }
        }
        public void Stop()
        {
            cancellation?.Cancel();
            scanner.DisConnect();
            biaoQianViewModel.print?.DisConnect();
            heGeViewModel.print?.DisConnect();
        }
        #endregion
        public HomeBll(IContainerExtension container, IEventAggregator eventAggregator) 
        {
            this.container = container;
            this.eventAggregator = eventAggregator;
            client = this.container.Resolve<IPLCHelper>();
            scanner = this.container.Resolve<HoneywellTCPScanner>();
            heGeViewModel = this.container.Resolve<HeGeViewModel>();
            biaoQianViewModel = this.container.Resolve<BiaoQianViewModel>();
        }
    }
}
