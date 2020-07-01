using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : MonoBehaviour, IAttack
{


    public List<GameObject> HitTargets(Transform attackPosition, float radius)
    {
        List<GameObject> hitTargets = new List<GameObject>();
        foreach(Collider collider in Physics.OverlapSphere(attackPosition.position, radius))
        {
            hitTargets.Add(collider.gameObject);
        }
        return hitTargets;
    }

    //public void DamageTargets(Transform attackPosition, float radius, int damage)
    //{
    //    foreach (Collider collider in Physics.OverlapSphere(attackPosition.position,radius))
    //    {
    //        //Debug.Log(collider.gameObject.name);
    //        if (collider != null && collider.gameObject.GetComponent<HealthHandler>() != null)
    //            collider.gameObject.GetComponent<HealthHandler>().ChangeHealth(-damage);
    //    }
    //}

    void DealDamage(List<GameObject> hitTargets, int damage)
    {
        foreach (GameObject go in hitTargets)
        {
            if (GetComponent<HealthHandler>() != null)
            {
                Debug.Log($"Dealt {damage} damage to {go.name}");
                go.GetComponent<HealthHandler>().ChangeHealth(-damage, GetComponent<Character>());
            }
        }
    }

    public void Attack(Transform attackPoint, float radius, int damage)
    {
        //Debug.Log(attackPoint.position);
        //Character character = gameObject.GetComponent<Character>();
        //character.Move(Vector3.forward * 100, character.baseSpeed);
        //GetComponent<Character>().DoMoveForSetTime(transform.forward, 10f, 0.19f);
        //gameObject.transform.Translate(Vector3.forward * 0.5f);
//        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
       // rb.velocity = Vector3.forward * 10;
        //print(gameObject.name);
        List<GameObject> hitTargets = HitTargets(attackPoint, radius);
        DealDamage(hitTargets, damage);
        
        //Debug.Log(HitTargets(attackPoint, radius).Count);
        //DamageTargets(attackPoint, radius, damage);
        //throw new System.NotImplementedException();
    }


}
