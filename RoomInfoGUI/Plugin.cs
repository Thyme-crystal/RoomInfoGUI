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
        private GUIStyle textstyle;

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

            if (textstyle == null)
            {
                textstyle = new GUIStyle(GUI.skin.label);
                textstyle.fontSize = 12;
                textstyle.alignment = TextAnchor.MiddleCenter;
                textstyle.fontStyle = FontStyle.Bold;
                textstyle.normal.textColor = Color.white;
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

                    foreach (VRRig rig in VRRigCache.ActiveRigs)
                    {
                        if (rig == null)
                            return;

                        string platform = Platform(rig);

                        string platformCL = platform;

                        if (platform.Contains("STEAM"))
                        {
                            platformCL = "<color=blue>" + platform + "</color>";
                        }


                        if (platform.Contains("QUEST"))
                        {
                            platformCL = "<color=cyan>" + platform + "</color>";
                        }

                        if (platform.Contains("PC?"))
                        {
                            platformCL = "<color=yellow>" + platform + "</color>";
                        }

                        string text = $"{PlayerListMSG}{rig.creator.NickName} [{platformCL}]";

                        if (rig.creator.IsMasterClient)
                        {
                            text += " <color=red>[Master]</color>";
                        }

                        if (rig.creator.IsLocal)
                        {
                            text += " <color=green>[You]</color>";
                        }

                        GUILayout.Label(text, textstyle);
                    }

                }
                else
                {
                    GUILayout.Label("N/A", textstyle);
                }
                GUILayout.EndVertical();
            }

            if (RoomINFO)
            {
                GUILayout.BeginVertical(boxStyle);

                if (PhotonNetwork.InRoom)
                {
                    GUILayout.Label(PhotonNetwork.CurrentRoom.Name, textstyle);
                    GUILayout.Label(PhotonNetwork.CurrentRoom.PlayerCount.ToString() + " Players.", textstyle);
                    GUILayout.Label("Master: " + PhotonNetwork.MasterClient.ToString(), textstyle);
                    GUILayout.Label("Region: " + PhotonNetwork.CloudRegion, textstyle);
                }
                else
                {
                    GUILayout.Label("Not in a Room.", textstyle);
                }

                RoomCode = GUILayout.TextField(RoomCode);

                if (GUILayout.Button("Join"))
                {
                    RoomCode = RoomCode.ToUpper();
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(RoomCode, JoinType.Solo);
                }

                GUILayout.EndVertical();
            }
        }

        public static string Platform(VRRig rig)
        {
            string platform = rig._playerOwnedCosmetics.Concat();

            if (platform.Contains("S. FIRST LOGIN"))
            {
                return "STEAM";
            }
            else if (platform.Contains("FIRST LOGIN"))
            {
                return "PC?";
            }
            else
            {
                return "QUEST";
            }
        }

        public static bool PlayerList = false;
        public static bool RoomINFO = false;
        public static string PlayerListMSG = " ";
        public static string RoomCode = "Room Code";
    }
}
