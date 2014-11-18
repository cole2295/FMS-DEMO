using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace WindowsServiceFactory
{
    [RunInstaller(true)]
    public partial class InstallerServiceFactory : Installer
    {
        public InstallerServiceFactory()
        {
            InitializeComponent();
        }
    }
}
