using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Iisu;
using Iisu.Data;


namespace Gateway_DDS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // iisu handle
        private IHandle iisuHandle;
 
        // iisu device
        private IDevice device;
 
        // validates iisu DataHandles
        private bool m_valid = false;
 
 
        // ensures we get a new frame
        private int m_lastFrameID = -1;
 
        // variables we're concerned with
        private IDataHandle<float> zoomStage;
        private IDataHandle<bool> hand1_closed;
        private IDataHandle<bool> hand2_closed;


        public MainWindow()
        {
            InitializeComponent();


            IContext context = Iisu.Iisu.Context;

            // get iisu handle
            iisuHandle = context.CreateHandle();

            // create iisu device
            device = iisuHandle.InitializeDevice();

            try
            {
                // register event listener
                device.EventManager.RegisterEventListener("SYSTEM.Error", new OnErrorDelegate(onError));

                // launch IID script
                device.CommandManager.SendCommand("IID.loadGraph", "gateway-v2.iid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

            CompositionTarget.Rendering += UpdateColor;

            device.Start();
            //while (true)
            {
                //update();
            }

        }

        int cnt = 0;
        int ccnt = 0;
        protected void UpdateColor(object sender, EventArgs e)
        {
            frameid.Text = "Frame count:" + cnt.ToString();
                cnt++;
            update();
        }


        public void update()
        {
            // make sure that iid parameters are registered
            registerIIDData();

            // only update if object is active (all parameters are properly registered)
            //if (!m_valid)
            //{
             //   return;
            //}

            // the rest of the logic depends on iisu data, so we need to make sure that we have
            // already a new data frame
            int currentFrameID = device.FrameId;
            //if (currentFrameID == m_lastFrameID)
            {
              //  return;
            }

            // remember current frame id
            m_lastFrameID = currentFrameID;
            cameraframe.Text = "Camera Frame: " + currentFrameID.ToString() + " of " + ccnt.ToString();
            ccnt++;

            // update iisu data
            feedback.Text = zoomStage.Value.ToString();

            device.ReleaseFrame();
            device.UpdateFrame(true);
        }



        private void registerIIDData()
        {
            if (m_valid)
            {
                return;
            }

            try
            {
                zoomStage = device.RegisterDataHandle<float>("IID.Script.zoomStage");
                hand1_closed = device.RegisterDataHandle<bool>("IID.Script.hand1_closed");
                hand2_closed = device.RegisterDataHandle<bool>("IID.Script.hand2_closed");
                   
                m_valid =  zoomStage.Valid && hand1_closed.Valid && hand2_closed.Valid;
            }
            catch (Exception e)
            {
                error.Text = "Failed to register iisu data: " + e.Message;
            }
        }







        private delegate void OnErrorDelegate(String name, Iisu.Error e);
        private void onError(String name, Iisu.Error e)
        {
            Console.WriteLine("OH NO!" + e.Message);
        }
    }
}
