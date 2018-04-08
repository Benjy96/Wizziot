using UnityEngine;

public class NPCAim : MonoBehaviour {

    Transform player;
    float inaccuracy;

	// Use this for initialization
	void Start () {
        player = PlayerManager.Instance.player.GetComponent<Transform>();
        //Harder difficulty means lower variation in placement position
        inaccuracy = 10 - (10 * GameMetaInfo._DIFFICULTY_SCALE);

        transform.position = player.position +
            new Vector3(Random.Range(-inaccuracy, inaccuracy), player.position.y, Random.Range(-inaccuracy, inaccuracy));
	}

    private void Update()
    {
        Vector3 playerPos = PlayerManager.Instance.player.transform.position;
        transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime);
    }
}
