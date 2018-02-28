using System;
using System.Windows.Forms;
using System.Reflection;

namespace AICheckers
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            Console.WriteLine(@"{0} Version {1}", AssemblyTitle, AssemblyVersion);
            Console.WriteLine(AssemblyCopyright);
            Console.WriteLine(AssemblyDescription);
            Console.WriteLine();
        }
      

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            boardPanel2.Invalidate();
        }

        #region Assembly Attribute Accessors

        private string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        private string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        private string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion

    }
}
