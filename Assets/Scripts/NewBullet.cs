using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using System.Linq;
using System.IO;
using UnityEngine.Rendering;

public class NewBullet : MonoBehaviour
{
    private bool canRun = false;
    [SerializeField] List<Bullet> bullets = new List<Bullet>();

    //[SerializeField] private string newBulletTextureName;
    //[SerializeField] private string newBulletAudioName;
    //[SerializeField] private string newBulletType;
    private AudioSource bulletSoundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //bullets = .ToList();
        //GameObject[] bulletBuffer = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(Bullet));
        //Debug.Log(bulletBuffer.Length);
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        foreach (GameObject bullet in FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (bullet.GetComponent<Bullet>() != null)
            {
                bullets.Add(bullet.GetComponent<Bullet>());
            }
        }

        List<AudioSource> soundPlayers = FindObjectsOfType<AudioSource>().ToList();
        foreach (AudioSource soundPlayer in soundPlayers)
        {
            if (soundPlayer.gameObject.name == "RangedAttackSource")
            {
                bulletSoundPlayer = soundPlayer;
            }
        }
        canRun = true;
    }


    IEnumerator DelayRun(string newBulletType, string newBulletTextureName, string newBulletAudioName)
    {
        yield return new WaitForFixedUpdate();
        ChangeBullet(newBulletType, newBulletTextureName, newBulletAudioName);
    }
    // Update is called once per frame
    public void ChangeBullet(string newBulletType, string newBulletTextureName, string newBulletAudioName)
    {
        if (canRun)
        {


            if (bullets.Count > 0)
            {

                // Old individual loading system

                //string directory = Path.Combine(Application.streamingAssetsPath, newBulletType);
                //Sprite newSprite = null;
                //AudioClip audioClip = null;
                //if (Directory.Exists(directory))
                //{
                //    if (File.Exists(Path.Combine(directory, newBulletTextureName)))
                //    {
                //        string spritePath = Path.Combine(directory, newBulletTextureName);
                //        byte[] spriteBytes = File.ReadAllBytes(spritePath);
                //        Texture2D texture = new Texture2D(2, 2);
                //        texture.LoadImage(spriteBytes);
                //        newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0), texture.height);

                //        //spriteRenderer.sprite = newSprite;
                //    }
                //    else
                //    {
                //        Debug.LogError(newBulletTextureName + " file not found in [StreamingAssets/" + newBulletType + "] folder. Please make sure the file exists and is named correctly.");
                //    }

                //    if (File.Exists(Path.Combine(directory, newBulletAudioName)))
                //    {
                //        string audioPath = Path.Combine(directory, newBulletAudioName);
                //        byte[] audioData = File.ReadAllBytes(audioPath);
                //        float[] floatArray = new float[audioData.Length / 2];
                //        for (int i = 0; i < floatArray.Length; i++)
                //        {
                //            short bitValue = System.BitConverter.ToInt16(audioData, i * 2);
                //            floatArray[i] = bitValue / 32768f;
                //        }

                //        audioClip = AudioClip.Create("AudioClip", floatArray.Length, 1, 44100, false);
                //        audioClip.SetData(floatArray, 0);
                //    }
                //    else
                //    {
                //        Debug.LogError(newBulletAudioName + " file not found in [StreamingAssets/"+ newBulletType+"] folder. Please make sure the file exists and is named correctly.");
                //    }


                //}
                //else
                //{
                //    Debug.LogError(newBulletType + " folder not found in [StreamingAssets] folder");
                //}

                
                // New Asset Bundle Loading system

                string path = Path.Combine(Application.streamingAssetsPath, "Bullets", newBulletType);


                AssetBundle newBullet = null;
                if (File.Exists(path))
                {
                    newBullet = AssetBundle.LoadFromFile(path);
                }
                else
                {
                    Debug.LogError("No Asset Bundle of the name "+newBulletType+" exists in"+ Path.Combine(Application.streamingAssetsPath, "Bullets"));
                }
                Sprite newSprite = null;
                AudioClip audioClip = null;

                if (newBullet != null)
                {
                    Texture2D spriteTexture = newBullet.LoadAsset<Texture2D>(newBulletTextureName);
                    newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0), spriteTexture.height);

                    audioClip = newBullet.LoadAsset<AudioClip>(newBulletAudioName);

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
        else
        {
            Debug.LogError("ChangeBullet() can not be run yet as the variables are still being set. Delaying start until variables are set");
            StartCoroutine(DelayRun(newBulletType, newBulletTextureName, newBulletAudioName));
        }

    }
}
