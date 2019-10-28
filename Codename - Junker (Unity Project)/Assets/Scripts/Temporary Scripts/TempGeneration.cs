using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGeneration : MonoBehaviour
{
    public List<GameObject> bodyoptions;
    private bool scrolling = false;
    private float cooldownTime = 0.2f;
    private bool onCooldown = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            scrolling = !scrolling;
        }

        if(scrolling && !onCooldown)
        {
            onCooldown = true;

            GameObject[] guns = GameObject.FindGameObjectsWithTag("gun");
            foreach (GameObject gun in guns)
                GameObject.Destroy(gun);

            GameObject temp = Instantiate(bodyoptions[Random.Range(0, bodyoptions.Count)], gameObject.transform);
            temp.transform.position = Vector3.zero;

            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        onCooldown = false;
    }
}
