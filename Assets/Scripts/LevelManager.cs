﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using GameAnalyticsSDK;



public class LevelManager : MonoBehaviour
{
    public static int TotalMapsNumber;
    [SerializeField] int AllMaps;
   public enum lvlState {Menu,Play,Fin};
    public GameObject StartPage;
    public GameObject FinPage;

    private MapData MapData;

    static public int lvlNum= 0;
    public static lvlState State;
     public Animator DarkScreen;
     public Light Light;
     public GrassColorer Grass;
    // ParticleSystem Balloons;

    
    public static float TotalBrightness;
    [Space][SerializeField]float ModelsBright;

     
    void Awake(){
        LocalizationService.Instance.Load();
        LocalizationService.Instance.SetLang("en");
                // lvlShow.SetActive(true);

    }
    void Start()
    {
        TotalBrightness = ModelsBright;
        TotalMapsNumber = AllMaps;
        SaveLoad.Load();
        MapData = GetComponentInChildren<MapData>();
        State = lvlState.Menu;
        if( SaveLoad.savedGame.lvl!=0)
           {lvlNum= SaveLoad.savedGame.lvl;}
        else {lvlNum = 1;}

//градиент травы
            if( SaveLoad.savedGame.GColor!=0)
            {GrassColorer.GrassColorNum = SaveLoad.savedGame.GColor;
            GrassColorer.GradStage = SaveLoad.savedGame.GGradStg;}
        else {GrassColorer.GrassColorNum=0;
            GrassColorer.GradStage=0;} 

        // Balloons = Camera.main.transform.parent.GetComponentInChildren<ParticleSystem>();

    }


    void FixedUpdate()
    {   
        if(TotalBrightness != ModelsBright){
            TotalBrightness = ModelsBright;
        }


        if (State == lvlState.Menu&&!StartPage.activeSelf){
            // DarkScreen.SetTrigger("Appear");
            StartPage.SetActive(true);

            MapData.LoadMap();

            //Не забываем записать прогрессию
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"level {lvlNum}");


        }


        if(State == lvlState.Fin){
           
           if(!FinPage.activeSelf){
                FinPage.SetActive(true);

            SaveLoad.savedGame.lvl = lvlNum + 1;
            SaveLoad.savedGame.map = MapData.mapNum + 1;
            SaveLoad.Save();
                Invoke("OnLvlCompleted",0.5f);
           }
            if (DarkScreen.GetComponent<DarkScreenControl>().Dark){
                ToNextLevel();
            }
        }
       
    }

    void OnLvlCompleted(){
        Vibration.Vibrate(1000);
        
        // Balloons.Play();
    }

    public void Play(){
        State = lvlState.Play;
        SwipeControl.ResetFp();
        SwipeControl.BlockSwipeInput();
        StartPage.SetActive(false);
    }
    //     void AllowSwipeInput(){
    //     SwipeControl.AllowSwipeInput();
    // }
    public void GetRewards(){
            DarkScreen.SetTrigger("Disappear");
            // ToNextLevel();
            

    }
    


    void ToNextLevel(){
            // Balloons.Stop();
            // Balloons.Clear();
            
            //Записываем прогрессию
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"level {lvlNum}");
            MapData.DestroyMap();
            State = lvlState.Menu; 
            lvlNum++;   
            FinPage.SetActive(false);
    


    }

    public void Revert(){
        SaveLoad.savedGame = new GameData();
        File.Delete(Application.persistentDataPath + "/savedGame.gd");
        SceneManager.LoadScene("game");
    }




}

