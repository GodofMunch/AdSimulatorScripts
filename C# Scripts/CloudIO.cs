using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using TMPro;

public class CloudIO : MonoBehaviour

{
    public static CloudIO instance { get; set; }
    private string saveName = "AdSimSave";
    private bool saving;
    private bool cloudLoaded;
    private Gems gems;
    private Score score;
    private string saveString;
    public TextMeshProUGUI savedInfo;
    private bool Authenticated {
        get {
            return Social.Active.localUser.authenticated;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gems = FindObjectOfType<Gems>();
        score = FindObjectOfType<Score>();
    }
    
    public void ReadSavedGame(string filename, 
        Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
        
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename, 
            DataSource.ReadCacheOrNetwork, 
            ConflictResolutionStrategy.UseLongestPlaytime, 
            callback);
    }
    
    public void WriteSavedGame(ISavedGameMetadata game, byte[] savedData, 
        Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
        
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved at: " + System.DateTime.Now);
        
        // You can add an image to saved game data (such as as screenshot)
        // byte[] pngData = <PNG AS BYTES>;
        // builder = builder.WithUpdatedPngCoverImage(pngData);
        
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
    }

    public void WriteUpdatedScore() {
        // Local variable
        ISavedGameMetadata currentGame = null;

        // CALLBACK: Handle the result of a write
        Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback = 
        (SavedGameRequestStatus status, ISavedGameMetadata game) => {
            Debug.Log("Saved Game Write: " + status);
        };

        // CALLBACK: Handle the result of a binary read
        Action<SavedGameRequestStatus, byte[]> readBinaryCallback = 
        (SavedGameRequestStatus status, byte[] data) => {
            Debug.Log("Saved Game Binary Read: " + status);
            if (status == SavedGameRequestStatus.Success) {
                // Read score from the Saved Game
                int score = 0;
                try {
                    string scoreString = System.Text.Encoding.UTF8.GetString(data);
                    Score.score = int.Parse(scoreString.Split(';')[1]);
                    gems.setGems(int.Parse(scoreString.Split(';')[0]));
                } catch (Exception e) {
                    Debug.Log("Saved Game Write: convert exception");
                }
                
                // Increment score, convert to byte[]
                int newScore = score + Score.score;
                string newScoreString = Convert.ToString(newScore);
                byte[] newData = System.Text.Encoding.UTF8.GetBytes(newScoreString);
                
                // Write new data
                Debug.Log("Old Score: " + score);
                Debug.Log("mHits: " + Score.score);
                Debug.Log("New Score: " + newScore);
                WriteSavedGame(currentGame, newData, writeCallback);
            }
        };

        // CALLBACK: Handle the result of a read, which should return metadata
        Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback = 
        (SavedGameRequestStatus status, ISavedGameMetadata game) => {
            Debug.Log("(Lollygagger) Saved Game Read: " + status.ToString());
            if (status == SavedGameRequestStatus.Success) {
                // Read the binary game data
                currentGame = game;
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, 
                                                    readBinaryCallback);
            }
        };

        // Read the current data and kick off the callback chain
        Debug.Log("(Lollygagger) Saved Game: Reading");
        ReadSavedGame("file_total_hits", readCallback);
    }
  
    // Update is called once per frame
    void Update()
    {
        
    }
}