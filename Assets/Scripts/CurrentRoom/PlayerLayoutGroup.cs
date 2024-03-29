﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour {

    [SerializeField]
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return _playerListingPrefab; }
    }

    private List<PlayerListing> _playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings
    {
        get { return _playerListings; }
    }

    //Called by photon whenever the master client is switched.
    private void OnMasterCleintSwitched(PhotonPlayer newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }

    //Called by Photon whenever you join a room.
    private void OnJoinedRoom()
    {
        MainCanvasManager.Instance.CurrentRoomCanvas.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        MainCanvasManager.Instance.LobbyCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 1.0f, 1.0f);
        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    //Called by photon when a player joins the room.
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }

    //Called by photon when a player leaves the room.
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }

    private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)
            return;

        PlayerLeftRoom(photonPlayer);

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);

        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public void OnClickRoomState()
    {
        if (PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MainCanvasManager.Instance.CurrentRoomCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 1.0f, 1.0f);
        MainCanvasManager.Instance.LobbyCanvas.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }
}
