using System.ComponentModel;
using System.Reflection;
using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RoomInfoGUI
{
    [Description("shows info of your current room")]
    [BepInPlugin("pilo.thyme.gorillatag.RoomGUI", "pilo.thyme.gorillatag.RoomGUI", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private Texture2D bgTex;
        private GUIStyle boxStyle;

        public Plugin()
        {
            new Harmony("thyme.RoomGUI").PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Awake()
        {
            bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.8f));
            bgTex.Apply();
        }

        public void OnGUI()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.fontSize = 18;
                boxStyle.alignment = TextAnchor.MiddleCenter;
                boxStyle.normal.textColor = Color.black;
                boxStyle.normal.background = bgTex;
            }

            GUILayout.BeginHorizontal(boxStyle);

            if (GUILayout.Button("Player List", boxStyle))
            {
                PlayerList = !PlayerList;
            }

            if (GUILayout.Button("Room Info", boxStyle))
            {
                RoomINFO = !RoomINFO;
            }

            if (GUILayout.Button("D", boxStyle))
            {
                PhotonNetwork.Disconnect();
            }

            GUILayout.EndHorizontal();

            if (PlayerList)
            {
                GUILayout.BeginVertical(boxStyle);
                if (PhotonNetwork.InRoom)
                {

                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        string text = PlayerListMSG + player.NickName;

                        if (player.IsMasterClient)
                            text += " <color=red>[Master]</color>";

                        if (player.IsLocal)
                            text += " <color=green>[You]</color>";
                        GUILayout.Label(text);
                    }
                }
                else
                {
                    GUILayout.Label("N/A");
                }
                GUILayout.EndVertical();
            }

            if (RoomINFO)
            {
                GUILayout.BeginVertical(boxStyle);

                if (PhotonNetwork.InRoom)
                {
                    GUILayout.Label("Room Info!");
                    GUILayout.Label(PhotonNetwork.CurrentRoom.Name);
                    GUILayout.Label(PhotonNetwork.CurrentRoom.PlayerCount.ToString());
                    GUILayout.Label(PhotonNetwork.MasterClient.ToString());
                    GUILayout.Label(PhotonNetwork.CloudRegion);
                }
                else
                {
                    GUILayout.Label("N/A");
                }

                GUILayout.EndVertical();
            }
        }
        public static bool PlayerList = false;
        public static bool RoomINFO = false;
        public static string PlayerListMSG = " ";
    }
}
