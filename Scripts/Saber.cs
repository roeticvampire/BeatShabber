using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{ public LayerMask layer;
    private Vector3 previouspos;
    public LineRenderer lr;
    public Transform start;
    AudioSource ad;
    public AudioClip[] hitsounds;
    Vector3 home = new Vector3(0, 0.4f, 0);
    public Material cross;

    


    // Start is called before the first frame update
    void Start()
    {
        ad = GetComponent<AudioSource>();
        //Cursor.visible = false;
        lr.SetPosition(0, start.position);
    }

    // Update is called once per frame
    void Update()
    { 
        lr.SetPosition(1, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(home, transform.position, out hit, 15, layer))
        {
            if (Vector3.Angle(transform.position - previouspos, -hit.transform.up) > 105)

            {
                Example.totalHits++;

                //playSound
                ad.PlayOneShot(hitsounds[Random.Range(0, hitsounds.Length)]);

               // GameObject[] slices = Slice(hit.transform.gameObject, hit.point, Vector3.Cross(hit.point, transform.position - previouspos));
                int disp = 2;
                GameObject c = hit.transform.gameObject;
               // foreach (GameObject c in slices)
                {

                    Destroy(c.gameObject);
                    // c.transform.position = hit.point+ new Vector3(0,0,6);
                    // slice.transform.rotation = hit.transform.rotation;
                    //__________________________________________________________

                    SlicedHull hull = c.gameObject.Slice(hit.transform.position, Vector3.Cross(hit.point, transform.position - previouspos));
                    print(hull);
                    if (hull != null)
                    {
                        GameObject lower = hull.CreateLowerHull(c.gameObject, cross);
                        GameObject upper = hull.CreateUpperHull(c.gameObject, cross);
                        GameObject[] objs = new GameObject[] { lower, upper };

                        foreach (GameObject o in objs)
                        {
                            o.transform.position = hit.transform.position+new Vector3(0,0,2);
                            o.transform.rotation = hit.transform.rotation;
                            o.transform.localScale = new Vector3(1, 1, 1);
                            o.AddComponent<Rigidbody>();
                            //Because it is an irregular object after cutting, so choose MeshCollider (mesh collision)
                            //If a MeshCollider is a rigid body, if you want to collide normally, you must set convex to true
                            //Unity's rules: This will form a convex polyhedron, only convex polyhedrons can be rigid
                            o.AddComponent<MeshCollider>().convex = true;
                            Object.Destroy(o, 5f);
                        }
                    }




















                    //______________
                    // slice.AddComponent<Rigidbody>();
                    // slice.GetComponent<Rigidbody>().AddExplosionForce(100f,hit.transform.position,20);
                    // disp *= -1;
                }
                //Destroy(hit.transform.gameObject);
                //  Debug.Log("Tod diye ek");}



            }
        }
            previouspos = transform.position;


        }
        
    } 
