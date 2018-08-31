using System;
using UnityEngine;
using KSP.IO;
using UnityEngine.UI;
using KSP;
using KSP.UI.Screens;

namespace MyFirstKSPModule
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Park : MonoBehaviour
    {
        private static ApplicationLauncherButton appLaunchButton;
        private static Texture2D appLauncherIcon = new Texture2D(38, 38);

        private void AddToStockAppLauncher() {
            if (appLaunchButton == null) {
                Callback onTrue = new Callback(onShowGUI);
                Callback onFalse = new Callback(onHideGUI);

                appLaunchButton = ApplicationLauncher.Instance.AddModApplication(
                    onTrue,
                    onFalse,
                    null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    appLauncherIcon
                );
            }
        }

        public void Start()
        {

            GameEvents.onGUIApplicationLauncherReady.Add(AddToStockAppLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(delegate {
                if (appLaunchButton != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(appLaunchButton);
                }
            });
        }

        private bool displayGUI = false;
        private Rect windowPostition = new Rect(20f, 100f, 250f, 150f);
        private void onShowGUI() {
            Debug.Log("PARK_MOD::DEBUG: onShowGUI");
            displayGUI = true;
        }

        private void onHideGUI()
        {
            Debug.Log("PARK_MOD::DEBUG: onHideGUI");
            displayGUI = false;
        }

        private static ITargetable targetedVessel;
        private static ITargetable activeVessel;
        private void OnWindow(int id)
        {
            GUIStyle label = new GUIStyle(GUI.skin.button);
            label.fixedWidth = 100f;

            GUILayout.BeginVertical();
            // Debug.Log("PARK_MOD::DEBUG: OnWindow");
            if (targetedVessel != null)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("TRG VSL", label);
                GUILayout.Label(targetedVessel.GetDisplayName());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("TRG V", label);
                GUILayout.Label(FlightGlobals.fetch.vesselTargetDelta.ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("TRG Orb V", label);
                GUILayout.Label(targetedVessel.GetObtVelocity().ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("ACT Orb V", label);
                GUILayout.Label(activeVessel.GetObtVelocity().ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("REL V", label);
                GUILayout.Label(relVelocityTarget.ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("REL Vm", label);
                GUILayout.Label(relVelocityTarget.magnitude.ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("ACT V", label);
                GUILayout.Label(activeVessel.GetObtVelocity().ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(250f));
                GUILayout.Label("FWD V", label);
                GUILayout.Label(activeVessel.GetFwdVector().ToString());
                GUILayout.EndHorizontal();
            } else {
                GUILayout.Label("No Vessel Targeted");
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public void OnGUI()
        {
            if (displayGUI)
                windowPostition = GUILayout.Window(2000, windowPostition, OnWindow, "This Is My Title");
        }

        public void Awake()
        {
            byte[] arrBytes;
            arrBytes = KSP.IO.File.ReadAllBytes<Park>("TeamRedLogo.png", null);
            Debug.Log(arrBytes.ToString());
            appLauncherIcon.LoadImage(arrBytes);
        }

        private static Vector3 relVelocityTarget = new Vector3();
        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.isEVA && !MapView.MapIsEnabled) {
                targetedVessel = FlightGlobals.fetch.VesselTarget;
                activeVessel = FlightGlobals.ActiveVessel;

                if (targetedVessel != null)
                    relVelocityTarget = targetedVessel.GetObtVelocity() - activeVessel.GetObtVelocity();
            }
        }



    }
}
