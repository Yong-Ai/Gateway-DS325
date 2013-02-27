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
 
        // are we bowing correctly?
        private bool is_flip = false;
 
        // ensures we get a new frame
        private int m_lastFrameID = -1;
 
        // variables we're concerned with
        private IDataHandle<bool> isflip;
        private IDataHandle<float> zoomStage;


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
                device.CommandManager.SendCommand("IID.loadGraph", "GateWay.iid");
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

        protected void UpdateColor(object sender, EventArgs e)
        {
            update();
        }


        public void update()
        {
            // make sure that iid parameters are registered
            registerIIDData();

            // only update if object is active (all parameters are properly registered)
            if (!m_valid)
            {
                return;
            }

            // the rest of the logic depends on iisu data, so we need to make sure that we have
            // already a new data frame
            int currentFrameID = device.FrameId;
            if (currentFrameID == m_lastFrameID)
            {
                return;
            }

            // remember current frame id
            m_lastFrameID = currentFrameID;

            // update iisu data
            //Console.WriteLine("left_hand_up: " + left_hand_up.Value + "     right_hand_up: " + right_hand_up.Value + "      is_bowing: " + is_bowing.Value);

            /*
            if (isflip.Value == true)
            {
                feedback.Text = "Flipping";
            } else {
                feedback.Text = "Not flipping";
            }
             * */

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
                m_valid =  zoomStage.Valid;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to register iisu data: " + e.Message);
            }
        }







        private delegate void OnErrorDelegate(String name, Iisu.Error e);
        private void onError(String name, Iisu.Error e)
        {
            Console.WriteLine("OH NO!" + e.Message);
        }
    }
}
