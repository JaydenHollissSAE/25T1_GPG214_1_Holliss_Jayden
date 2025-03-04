using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveMenuButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void SaveGame(int saveSlot)
    {
        PlayerCharacter playerCharacter = FindFirstObjectByType<PlayerCharacter>();
        GameObject checkpoint = playerCharacter.m_LastCheckpoint.gameObject;
        bool hasWeapon = playerCharacter.gameObject.GetComponent<Damager>().m_CanDamage;
        int health = playerCharacter.gameObject.GetComponent<Damageable>().m_CurrentHealth;

        SaveFile.saveManager.StartSave(checkpoint, hasWeapon, health, saveSlot, SceneManager.GetActiveScene().name);
    }

    public void LoadGame(int saveSlot)
    {
        SaveFile.saveManager.LoadSave(saveSlot);
    }

}
