using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : State
{
    float attackTime;
    bool attacking;
    
    public State_Attack(Character character) : base(character)
    {
        //attackTime = character.attackSpeed;

    }




    public override IEnumerator OnStateEnter()
    {
        attacking = true;
        attackTime = character.attackSpeed;
        float attackTimer = attackTime;
        //Debug.Log(attackTime.ToString());
        int attackCount = 0;

        //FOR IMPLEMENTATION BEFORE ITEM WORKS
        int maxAttackCount = 2;

        character.canAttack = false;
        Debug.Log(character.Name + " Entered Attack state");
        character.anim.SetBool("IsAttacking", true);
        attackCount++;
        character.anim.SetInteger("AttackCount", attackCount);
        character.DoMoveForSetTime(character.transform.forward, 10f, 0.19f);
        yield return new WaitForSeconds(0.19f);
        Attack();
        while (true)
        {

            //Play the attack animation

            // The weapon should retrieve a list of the hit targets

            //Wait for animation to finish
            attackTimer -= Time.deltaTime;
            //Debug.Log(attackTimer);

            
            //if(animation has finished running)
            //character.canAttack = true;
            if (attackTimer <= attackTime * 0.5f && attackCount < maxAttackCount)
                character.canAttack = true;
            //This should use the attackspeed of the character and the attackrate of the weapon to calculate.
            //Attackspeed == how many attacks the character can do each second
            //Attackspeed 1 = 1 attack each second
            //Attackspeed 2 = 2 attacks a second and so forth.
            //Weapon attack rate will influence the characters attack speed to calculate attacks per second
            //Attacks per second == character.attackSpeed / weapon.attackRate
            //If no weapon is equipped, the attacks per second will be the chaaracter.attackSpeed
            //In  the event of guns, the gun will have an attack rate and a fire rate.
            //fire rate are burst, auto and single fire.
            //The attack rate is the time from once the last bullet was fired until the gun can be fired again.




            //Does a new attack
            if (character.canAttack && attackCount < maxAttackCount)
            {
                if (character.characterType == Character.CharacterType.Player && Input.GetMouseButtonDown(0))
                {
                    attackTimer = attackTime;
                    attackCount++;
                    character.anim.SetInteger("AttackCount", attackCount);
                    character.DoMoveForSetTime(character.transform.forward, 10f, 0.19f);
                    yield return new WaitForSeconds(0.19f);
                    Attack();
                }
                else if (character.characterType == Character.CharacterType.NPC && character.currentTarget != null && Vector3.Distance(character.transform.position, character.currentTarget.position) < character.attackRange)
                {
                    //yield return new WaitForSeconds(0.19f);
                    Debug.Log("FOOO");
                    attackTimer = attackTime;
                    attackCount++;
                    character.anim.SetInteger("AttackCount", attackCount);
                    character.DoMoveForSetTime(character.transform.forward, 10f, 0.19f);
                    yield return new WaitForSeconds(0.19f);
                    Attack();
                }


                if(character.characterType == Character.CharacterType.NPC && character.currentTarget != null && Vector3.Distance(character.transform.position, character.currentTarget.position) > character.attackRange)
                {
                    character.GoToState(character.stateList[2]);
                }
                //else if(character.characterType == Character.CharacterType.NPC && character.currentTarget == null)
                //{
                //    character.GoToState(character.stateList[3]);
                //}


            }
            //if (character.canAttack && character.characterType == Character.CharacterType.NPC && attackCount == maxAttackCount)
            //{
            //    if(character.gameObject.GetComponent<NPC_CharacterController>().targets.Count > 0)
            //    {
            //        yield return new WaitForSeconds(0.35f);
            //        attackCount = 0;
            //    }
            //    //Debug.Log("FOO");
            //    //character.anim.SetInteger("AttackCount", attackCount);
            //}
            if (attackTimer <= 0)
            {
                //Debug.Log("FOO");
                //Set hasAttacked to true
                character.hasAttacked = true;
                attacking = false;
            }



            if (!attacking)
                break;
            yield return null;
        }

        //Go through list of hit targets and see if they should take damage
        //Deal damage
        //Debug.Log("BAR");
        if (character.characterType == Character.CharacterType.NPC)
            character.GoToState(character.stateList[0]);
        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        Debug.Log($"{character.Name} Exited Attack state");
        character.anim.SetBool("IsAttacking", false);
        attacking = false;
        character.hasAttacked = false;
        //character.canAttack = true;
        character.StartAttackCooldown();
        return base.OnStateExit();
    }

    void Attack()
    {
        character.canAttack = false;
        Debug.Log($"{character.name} Did a new attack");
        character.DoAttack(character.attackPoint, character.weaponAttackRadius, character.weaponDamage);
    }
}
