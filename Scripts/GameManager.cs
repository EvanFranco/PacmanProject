using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Ghost[] ghosts;

    public Pacman pacman;

    public Transform[] pellets;

    public int ghostMultiplier {get;private set;} = 1;
    // you see current score but cannot set and is set automatically
    public int score{ get; private set; }

    public int lives{ get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        NewGame();
    }

    private void Update(){
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Return)){
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    // each new round we restart pellet
    private void NewRound()
    {
        foreach(Transform pellet in this.pellets){
            pellet.gameObject.SetActive(true);
        }
        ResetState();
    }
    //reset ghost and pacman but not pellets 
    private void ResetState()
    {
        ResetGhostMultiplier();
        for (int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);

    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);

        if(this.lives > 0 ){
            Invoke(nameof(ResetState),3.0f);
        } else{
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);

        if (!HasRemainingPellet())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound),3.0f);

        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for(int i = 0; i < this.ghosts.Length;i++){
            this.ghosts[i].frightned.Enable(pellet.duration);
        }
        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier),pellet.duration);
    }

    private bool HasRemainingPellet()
    {
        foreach(Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf){
                return true;
            }   
        }
        return false;
    }
    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }
}


