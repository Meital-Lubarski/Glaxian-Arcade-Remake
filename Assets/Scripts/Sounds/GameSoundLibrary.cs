using UnityEngine;


[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class GameSoundLibrary : ScriptableObject
{
   [Header("Player Sounds")]
   public SoundData playerShoot;

   public SoundData playerDeath;


   [Header("Enemy Sounds")] 
   public SoundData enemyShoot;

   public SoundData enemyExplosion;

   public SoundData hitBoss;

   public SoundData divingSound;


   [Header("UI & Background")]
   public SoundData startGame;
   
   public SoundData fallingStar;
   
   public SoundData starExplosion;
   
   public SoundData backgroundMusic;
   

}



