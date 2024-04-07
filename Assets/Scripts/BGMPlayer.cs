using UnityEngine;

public class BGMPlayer : MonoSingleton<BGMPlayer>
{
    public AudioSource Audio;

    public AudioClip Hitoshizuku;
    public AudioClip Utakotoba;
    public AudioClip Katyusha;

    public int playingInt;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Audio.clip = Hitoshizuku;
        playingInt = 0;
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetInt("BGMVolume", 50);
        }
        Audio.volume = 0.01f * PlayerPrefs.GetInt("BGMVolume");
        Audio.Play();
    }

    void Update()
    {
        if (!Audio.isPlaying && Audio.time>=Audio.clip.length)
        {
            Debug.Log("Ëæ»ú");
            int randomNum = Random.Range(0, 2);
            switch (playingInt)
            {
                case 0:
                    SelectBGM(randomNum + 1);
                    break;
                case 1:
                    SelectBGM(randomNum * 2);
                    break;

                case 2:
                    SelectBGM(randomNum);
                    break;
            }
        }
    }

    public void SelectBGM(int input)
    {
        switch (input)
        {
            case 0:
                Audio.clip = Hitoshizuku;
                playingInt = 0;
                break;
            case 1:
                Audio.clip = Utakotoba;
                playingInt = 1;
                break;
            case 2:
                Audio.clip = Katyusha;
                playingInt = 2;
                break;
        }
        Audio.Play();
    }
}
