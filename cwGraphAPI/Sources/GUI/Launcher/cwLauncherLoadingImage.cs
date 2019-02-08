using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// Manage to Loading Animated Image
    /// </summary>
    /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
    /// <typeparam name="RootNodeType">The type of the OO t_ NODE.</typeparam>
    /// <typeparam name="CreateItemType">The type of the REAT e_ ITEM.</typeparam>
    public class cwLauncherLoadingImage<GuiType, RootNodeType, CreateItemType>
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {

        Form loadingForm = null;
        Thread loadingThread = null;
        cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwLauncherLoadingImage&lt;GuiType, RootNodeType, CreateItemType&gt;"/> class.
        /// </summary>
        /// <param name="mainGUI">The main GUI.</param>
        public cwLauncherLoadingImage(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
        {
            this.mainGUI = mainGUI;
        }

        /// <summary>
        /// Starts the loading image.
        /// </summary>
        public void startLoadingImage()
        {
            if (loadingThread != null && loadingThread.IsAlive)
            {
                return;
            }
            loadingThread = new Thread(new ThreadStart(workerThread));
            loadingThread.Start();
        }


        /// <summary>
        /// Disposes the loading form.
        /// </summary>
        public void disposeLoadingForm()
        {
            if (loadingForm != null && true.Equals(loadingThread.IsAlive))
            {
                loadingForm.Invoke(new MethodInvoker(stopLoadingThread));
            }
        }

        /// <summary>
        /// Stops the loading thread.
        /// </summary>
        private void stopLoadingThread()
        {
            loadingForm.Close();
        }

        /// <summary>
        /// Workers the thread.
        /// </summary>
        private void workerThread()
        {
            loadingForm = new cwLoadingForm();
            loadingForm.Size = new System.Drawing.Size(67, 69);
            loadingForm.StartPosition = FormStartPosition.Manual;
            loadingForm.Location = new Point(mainGUI.Location.X + mainGUI.Size.Width / 2 - loadingForm.Size.Width / 2, mainGUI.Location.Y + mainGUI.Size.Height / 2 - loadingForm.Size.Height / 2);
            loadingForm.BringToFront();
            loadingForm.TopMost = true;
            Application.Run(loadingForm);
        }

    }
}
