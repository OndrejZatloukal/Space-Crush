using UnityEngine;
using System.Collections;

public class SeekingManouver : EvasiveManouver
{
    private Transform playerTransform;

    protected override void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        base.Start();
    }

    protected override IEnumerator SetManouver()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));

        while (true)
        {
            if (playerTransform != null)
            {
                targetManouver = playerTransform.position.x;
            }
            yield return new WaitForSeconds(Random.Range(manouverTime.x, manouverTime.y));
            targetManouver = 0;
            yield return new WaitForSeconds(Random.Range(manouverWait.x, manouverWait.y));
        }
    }
}
