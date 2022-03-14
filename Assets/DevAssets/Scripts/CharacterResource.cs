using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResource : MonoBehaviour
{
    [Header("Health")]
    public float health = 100;
    public float healthMax = 100;
    [Header("Energy")]
    public float energy = 100;
    public float energyMax = 100;
    [Header("Mental Health")]
    public int mentalHealth = 10;
    public int mentalHealthMax = 10;
    [Header("Armor")]
    public int armor = 50;
    public int armorMax = 50;
    [Header("Bullets")]
    public int bullets = 25;
    public int bulletsMax = 250;
    public int reloadAmount = 25;
    public int bulletsPerRound = 5;
    [Header("Grenade")]
    public int grenade = 5;
    public int grenadeMax = 5;
    [Header("Actions Point")]
    public int actionsPoint = 5;
    public int actionsPointMax = 5;
}
