using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace tutoriales.multiplayer
{
    public class AutoLobby : MonoBehaviourPunCallbacks
{

        public Button ConnectButton;
        public Button JoinRandomButton;
        public Text Log;
        public Text PlayerCount;
        public int PlayersCount;

        public byte maxPlayersPerRoom = 4;
        public byte minPlayersPerRoom = 2;
        private bool IsLoading = false;

        public void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.ConnectUsingSettings())
                {
                    Log.text += "\nConnected to Server";
                }
                else
                {
                    Log.text += "\nFallo de conexión con servidor";
                }
                
                
            }
        }

        public override void OnConnectedToMaster()
        {
            ConnectButton.interactable = false;
            JoinRandomButton.interactable = true;

        }

        public void JoinRandom()
        {
            if (PhotonNetwork.JoinRandomRoom())
            {
                Log.text += "\nFail Joining Room";
            }
            
        }


        
     

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
            Log.text += "\nNo hay rooms creamos una nueva";

            if(PhotonNetwork.CreateRoom(null,new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayersPerRoom }))
            {
                Log.text += "\nRoom Created";
            }
            else
            {
                Log.text += "\nFail creating room";
            }
        }

        public override void OnJoinedRoom()
        {
            Log.text += "\nEmpecemos";
            //desactivar boton
            JoinRandomButton.interactable = false;
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.CurrentRoom != null)
            
                PlayersCount = PhotonNetwork.CurrentRoom.PlayerCount;
                PlayerCount.text = PlayersCount + "/" + maxPlayersPerRoom;

            if (!IsLoading && PlayersCount >= minPlayersPerRoom)
            {
                LoadMap();
            }
            
          
        }

        private void LoadMap()
        {
            IsLoading = true;

            PhotonNetwork.LoadLevel("Mapa");
        }
    }
}
