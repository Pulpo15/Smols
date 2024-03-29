using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public Camera mainCamera;

    public MatchSettings matchSettings;

    private void Awake() {
        if(instance != null) {
            Debug.LogError("GameManager is a singleton, can't be instantiated more than 1 times");
        } else 
            instance = this;

        mainCamera = Camera.main;
    }

    #region Player Tracking

    private const string PLAYER_ID_PREFIX = "Player";

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    public static void RegisterPlayer(string _netId, PlayerManager _player) {
        string _playerId = PLAYER_ID_PREFIX + _netId;
        players.Add(_playerId, _player);
        _player.transform.name = _playerId;
    }

    public static void UnRegisterPlayer(string _playerId) {
        players.Remove(_playerId);
    }

    public static PlayerManager GetPlayer (string _playerId) {
        return players[_playerId];
    }

    //private void OnGUI() {
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach (string _playerId in players.Keys) {
    //        GUILayout.Label(_playerId + "  -  " + players[_playerId].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
    #endregion
}
