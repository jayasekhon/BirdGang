using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;

    // Fixes issue with only latest room showing
	private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;
	[SerializeField] GameObject readyButton;
	[SerializeField] GameObject notReadyButton;

	private int numPlayersRdy = 0;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if (!PhotonNetwork.IsConnected)
		{
			Debug.Log("Connecting to Master");
			PhotonNetwork.ConnectUsingSettings();
		}

	}

	void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			startGameButton.SetActive(PhotonNetwork.IsMasterClient);
			return;
		}
			
		if (numPlayersRdy == PhotonNetwork.PlayerList.Length)
		{
			startGameButton.SetActive(PhotonNetwork.IsMasterClient);
		}
		else 
		{
			startGameButton.SetActive(false);
		}
	}
	
	[PunRPC]
	public virtual void IncrementPlayersReady()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			numPlayersRdy += 1;
		}
			
	}

	[PunRPC]
	public virtual void DecrementPlayersReady()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			numPlayersRdy -= 1;
		}
			
	}

	[PunRPC]
	public virtual void SendNewMasterReadyList(int numRdy)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			numPlayersRdy = numRdy;
		}
			
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
	}

	public void CreateRoom()
	{
		if(!CheckRoomNameValid(roomNameInputField.text))
		{
			return;
		}
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 6;
		PhotonNetwork.CreateRoom(roomNameInputField.text, options);
		MenuManager.Instance.OpenMenu("loading");
	}

	public static bool CheckRoomNameValid(string roomName)
	{
		if(string.IsNullOrEmpty(roomName))
		{
			return false;
		}
		else if (roomName.Length > 22)
		{
			return false;
		}
		else
			return true;
	}

	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;

		// Clears the player list so that players in previous rooms do not show
		foreach(Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for(int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}
	}

	public void ConfirmReady()
	{
		readyButton.SetActive(false);
		notReadyButton.SetActive(true);
		photonView.RPC("IncrementPlayersReady", RpcTarget.MasterClient);
	}

	public void NotReady()
	{
		notReadyButton.SetActive(false);
		readyButton.SetActive(true);
		photonView.RPC("DecrementPlayersReady", RpcTarget.MasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (numPlayersRdy == PhotonNetwork.PlayerList.Length)
		{
			startGameButton.SetActive(PhotonNetwork.IsMasterClient);
		}
		photonView.RPC("SendNewMasterReadyList", RpcTarget.All, numPlayersRdy);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}

	public void StartGame()
	{
		if (RoomMeetsStartGameRequirements(PhotonNetwork.CurrentRoom.PlayerCount))
		{		
			startGameButton.GetComponent<Button>().interactable = false; // Stop the button from being clicked twice.
			PhotonNetwork.LoadLevel(2);
        	PhotonNetwork.CurrentRoom.IsVisible = false;
		}
	}

	public static bool RoomMeetsStartGameRequirements(int numPlayersInRoom)
	{
		if (numPlayersInRoom >= 1 && numPlayersInRoom <= 6) // NEEDS TO CHANGE: once we are done doing our work we can make this requirement 3-6 :)
			return true;
		else
			return false; 
	}

	public void LeaveRoom()
	{
		if (!readyButton.activeSelf)
		{
			NotReady();
		}
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		cachedRoomList.Clear();
		MenuManager.Instance.OpenMenu("title");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach(Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}

		// Debug.Log("Number of rooms: "+roomList.Count);
		// for(int i = 0; i < roomList.Count; i++)
		// {
		// 	if(roomList[i].RemovedFromList)
		// 		continue;
		// 	Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		// }

		for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else if (info.PlayerCount == info.MaxPlayers)
            {
                cachedRoomList.Remove(info.Name);
                //continue;
            } 
            else if (!info.IsVisible)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
		foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList)
		{
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(cachedRoomList[entry.Key]);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
}