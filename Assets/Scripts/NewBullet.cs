using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using System.Linq;
using System.IO;

public class NewBullet : MonoBehaviour
{
    List<Bullet> bullets = new List<Bullet>();

    [SerializeField] private string newBulletTextureName;
    [SerializeField] private string newBulletAudioName;
    [SerializeField] private string newBulletType;
    private AudioSource bulletSoundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        bullets = FindObjectsOfType<Bullet>().ToList();
        List<AudioSource> soundPlayers = FindObjectsOfType<AudioSource>().ToList();
        foreach (AudioSource soundPlayer in soundPlayers)
        {
            if (soundPlayer.gameObject.name == "RangedAttackSource") 
            {
                bulletSoundPlayer = soundPlayer;
            }
        }
        
    }

    // Update is called once per frame
    void ChangeBullet()
    {

        if (bullets.Count > 0)
        {


            string directory = Path.Combine(Application.streamingAssetsPath, newBulletType);
            Sprite newSprite = null;
            AudioClip audioClip = null;
            if (Directory.Exists(directory))
            {
                if (File.Exists(Path.Combine(directory, newBulletTextureName)))
                {
                    string spritePath = Path.Combine(directory, newBulletTextureName);
                    byte[] spriteBytes = File.ReadAllBytes(spritePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(spriteBytes);
                    newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0), texture.height);

                    //spriteRenderer.sprite = newSprite;
                }
                else
                {
                    Debug.LogError(newBulletTextureName + " file not found in [StreamingAssets/" + newBulletType + "] folder. Please make sure the file exists and is named correctly.");
                }

                if (File.Exists(Path.Combine(directory, newBulletAudioName)))
                {
                    string audioPath = Path.Combine(directory, newBulletAudioName);
                    byte[] audioData = File.ReadAllBytes(audioPath);
                    float[] floatArray = new float[audioData.Length / 2];
                    for (int i = 0; i < floatArray.Length; i++)
                    {
                        short bitValue = System.BitConverter.ToInt16(audioData, i * 2);
                        floatArray[i] = bitValue / 32768f;
                    }

                    audioClip = AudioClip.Create("AudioClip", floatArray.Length, 1, 44100, false);
                    audioClip.SetData(floatArray, 0);
                }
                else
                {
                    Debug.LogError(newBulletAudioName + " file not found in [StreamingAssets/"+ newBulletType+"] folder. Please make sure the file exists and is named correctly.");
                }


            }
            else
            {
                Debug.LogError(newBulletType + " folder not found in [StreamingAssets] folder");
            }


            foreach (Bullet bullet in bullets)
            {
                GameObject bulletObject = bullet.gameObject;
                if (!bulletObject.active)
                {
                    if (newSprite != null)
                    {
                        bulletObject.GetComponent<SpriteRenderer>().sprite = newSprite;
                    }
                    if (audioClip != null)
                    {
                        bulletSoundPlayer.clip = audioClip;
                    }

                }
            }

        }
        else
        {
            Debug.LogError("No bullets found in scene to change");
        }

    }
}
