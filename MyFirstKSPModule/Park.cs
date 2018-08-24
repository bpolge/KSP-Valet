using System;
using UnityEngine;
using KSP.IO;
using UnityEngine.UI;
using KSP;
using KSP.UI.Screens;

namespace MyFirstKSPModule
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
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
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT,
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

        private bool _displayGUI = false;
        private Rect _windowPostition = new Rect(20f, 100f, 250f, 150f);
        private void onShowGUI() {
            Debug.Log("PARK_MOD::DEBUG: onShowGUI");
            _displayGUI = true;
        }

        private void OnWindow(int id)
        {
            GUIStyle label = new GUIStyle(GUI.skin.button);

            // Debug.Log("PARK_MOD::DEBUG: OnWindow");
            GUILayout.BeginHorizontal(GUILayout.Width(250f));
            GUILayout.Label("This is my Layout", label);
            GUILayout.Label("YESSIR");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(250f));
            GUILayout.Label("Another one", label);
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void onHideGUI() {
            Debug.Log("PARK_MOD::DEBUG: onHideGUI");
            _displayGUI = false;
        }

        public void OnGUI()
        {
            // Debug.Log("PARK_MOD::DEBUG: OnGUI");
            if (_displayGUI)
                _windowPostition = GUILayout.Window(1800, _windowPostition, OnWindow, "This Is My Title");
        }

        public void Awake()
        {
            byte[] arrBytes;
            arrBytes = KSP.IO.File.ReadAllBytes<Park>("TeamRedLogo.png", null);
            Debug.Log("!!!!!!!!!!!!!!!");
            Debug.Log(arrBytes.ToString());
            appLauncherIcon.LoadImage(arrBytes);
        }

        public void Update()
        {
            // Debug.Log("PARK_MOD::DEBUG: Upodate");
        }



    }
}
