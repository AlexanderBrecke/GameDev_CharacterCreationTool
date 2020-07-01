using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    private int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        //Here you would ideally run a function to find the current health that was last saved.
        //But since we're not saving anything at this moment we will just set current health to max health at start.
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int healthChange, Character attacker)
    {
        currentHealth += healthChange;

        if (healthChange < 0) 
        { 
            //FindDirectionFromAttacker(attacker);
            GetComponent<Character>().DoMoveForSetTime(FindDirectionFromAttacker(attacker), 25f, 0.1f);
        }
        
        //HasTakenDamage(healthChange, () =>  print( "FOO"));
        if (currentHealth <= 0)
            Die();
        else
        {
            if (GetComponent<NPC_CharacterController>() != null)
            {
                if (healthChange < 0)
                {
                    GetComponent<NPC_CharacterController>().HasBeenAttacked(attacker);
                }
            }
        }
    }

    public void Die()
    {
        //Need some function to make the character die.
        Destroy(gameObject);
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth; 
    }

    Vector3 FindDirectionFromAttacker(Character attacker)
    {
        Vector3 direction = (transform.position - attacker.transform.position).normalized;
        //print(direction);
        return direction;
    }

    //public bool HasTakenDamage(int healthChange, Action callback)
    //{
    //    bool hasTakenDamage = false;
    //    if (healthChange < 0)
    //        hasTakenDamage = true;
    //    return hasTakenDamage;
    //}




}
