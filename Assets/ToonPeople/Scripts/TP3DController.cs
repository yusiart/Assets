using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonPeople
{
    public class TP3DController : MonoBehaviour
    {
        public float tall;
        public float turnspeed = 5f;
        public float walkspeed = 1.4f;
        public float walkaceleration = 5f;
        public float runspeed = 3.5f;
        public float sprintspeed = 8f;
        public float jumpforce = 10f;
        public float flyspeed = 8f;
        public float aimzoom;

        public RuntimeAnimatorController MaleControllerAnimations;
        public RuntimeAnimatorController FemaleControllerAnimations;
        public RuntimeAnimatorController ElderControllerAnimations;

        GameObject Model;
        GameObject Helper;
        GameObject Target;
        GameObject lookat;
        //[HideInInspector]
        public Canvas Crosshair;
        Canvas MyCross;
        Transform trans;
        Transform Modtrans;
        Rigidbody rigid;
        Animator anim;
        float divergence;
        float diverF;
        float tospeed;
        float toAspeed;
        float speed;
        float Aspeed;
        float express;
        float grtime;
        bool floating;
        bool grounded;
        bool moving;
        bool aim; bool canaim;
        bool GlassesONOFF;
        bool stairs;
        bool slope;
        Vector3 InputMoveDir;
        Vector3 dirforw;
        Vector3 dirfall;
        Vector3 dorsal;
        float angleforward;
        float angleright;
        bool active;
        int idles;
        float idletime;
        float blocked;
        int stand;
        int newstand;
        float zoom;
        float zoomup;
        float zoomup2;
        Collider CharCol;
        float VInput;
        float HInput;
        bool visibility;
        Transform Head;
        Transform Pelvis;
        Transform Glasses;
        int LayerAim = 2;
        int LayerIdles = 1;

        public GameObject[] Males;
        public GameObject[] Females;
        public bool Male;
        public bool Female;
        public bool Elder;
        GameObject Character;


        private void Start()
        {
            SelectChar();
            stand = 1;
            canaim = true;
            active = true;
            floating = false;
            blocked = 1;
            idles = 0;
            visibility = true;
            aim = false;

            Helper = new GameObject("Helper");
            Target = new GameObject("Target");
            Helper.transform.position = transform.position + Vector3.up * (tall - (0.375f / tall));
            Helper.transform.parent = transform;
            Target.transform.position = transform.position + Vector3.up * tall;
            Target.transform.parent = Helper.transform;
            Camera.main.transform.position = transform.position + new Vector3(0f, 3f, -5f);
            Camera.main.transform.parent = Helper.transform;
            Camera.main.transform.LookAt(Target.transform);
            Camera.main.fieldOfView = 60f;
            zoom = 4f;
            zoomup = 1f;
            lookat = new GameObject("lookat");
            lookat.transform.parent = Target.transform;
            lookat.transform.position = Target.transform.position;

            trans = GetComponent<Transform>();
            Modtrans = Model.GetComponent<Transform>();
            rigid = GetComponent<Rigidbody>();
            anim = Model.GetComponent<Animator>();

            express = 0f;
            InputMoveDir = transform.forward;
            Helper.transform.Rotate(Vector3.up * 180);
            if (Model.GetComponent<CapsuleCollider>() != null) Model.GetComponent<CapsuleCollider>().enabled = false;
            CharCol = transform.GetComponent<CapsuleCollider>();
            MyCross = Instantiate(Crosshair);
            MyCross.GetComponent<Canvas>().enabled = false;
            Head = Model.transform.Find("ROOT/TP/TP Pelvis/TP Spine/TP Spine1/TP Spine2/TP Neck/TP Head");
            Glasses = Model.transform.Find("ROOT/TP/TP Pelvis/TP Spine/TP Spine1/TP Spine2/TP Neck/TP Head/Glasses");
            GlassesONOFF = Glasses.gameObject.activeSelf;
            Pelvis = Model.transform.Find("ROOT/TP/TP Pelvis");
            dirforw = Model.transform.forward;
            InputMoveDir = Model.transform.forward;
            dirforw = Model.transform.forward;
            InputMoveDir = Model.transform.forward;
        }
        private void Update()
        {
            //  1 stand up      2 crouch      3 sliding     4 ladders     5 sit down     6 floating     7 dive      8 climbing      9 flying      10 swim     11 pushing
            if (active) GetInput();
            MoveCam();
            if (stand == 1) // stand up
            {
                SetMoveDir1();
                if (active) MoveChar1();
                GetInput1();
                if (!floating) Gravity();
            }
            else if (stand == 3) //sliding
            {
                SetMoveDir3();
                GetInput3();
                MoveChar3();
            }
            else if (stand == 5) //sit down
            {
                GetInput5();
            }
            else if (stand == 6) //BedR
            {
                GetInput6();
            }
            else if (stand == 7) //BedL
            {
                GetInput7();
            }
            else if (stand == 8) //LieDown BeachTowel
            {
                GetInput8();
            }
        }

        private void FixedUpdate()
        {
            if (!floating) if (stand == 1) CheckGround();
        }

        // always 
        void GetInput()
        {          
            if (Input.GetKeyDown("o")) //reset position
            {
                zoomup = 1f;
                lookat.transform.parent = Target.transform;
                StopAllCoroutines();
                trans.position = new Vector3(0f, 8f, -10f);
                active = true;
                StopChar();
                stand = 1;
                grounded = false;
                anim.SetBool("grounded", false);
                anim.Play("freefall");
                trans.GetComponent<CapsuleCollider>().enabled = true;
            }           
            if (Input.GetKeyDown("r")) // randomize character
            {
                RandomizeChar();
            }
            if (Input.GetKeyDown("escape")) Application.Quit();
        }
        void MoveCam()
        {
            if (!aim)
            {
                //Camera position
                Vector3 ToPosition;
                Helper.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f);
                Target.transform.position = Helper.transform.position;
                Target.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0f, 0f));
                zoom -= Input.GetAxis("Mouse ScrollWheel");
                zoom = Mathf.Clamp(zoom, 0.75f, 11f);

                float myrad = 0.225f;
                if (Physics.SphereCast(Target.transform.position, myrad, -Target.transform.forward, out RaycastHit hit, zoom - myrad))
                    ToPosition = hit.point + (hit.normal * myrad);
                else ToPosition = Target.transform.position - (Target.transform.forward * (zoom - myrad));

                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, ToPosition, 0.025f);

                if (Mathf.Abs(zoomup - zoomup2) > 0.1f) zoomup2 = Mathf.Lerp(zoomup2, zoomup, 1f * Time.deltaTime);

                if ((lookat.transform.parent.transform.position - lookat.transform.position).magnitude > 0.125f)
                {
                    lookat.transform.position = Vector3.Lerp(lookat.transform.position, lookat.transform.parent.transform.position, 1f * Time.deltaTime);
                }

                Camera.main.transform.LookAt(lookat.transform.position + Vector3.up * (Camera.main.transform.position - Target.transform.position).magnitude * -0.125f * zoomup2);

                if ((Camera.main.transform.position - Target.transform.position).magnitude > 0.5f && !visibility) HideChar(true);
                if ((Camera.main.transform.position - Target.transform.position).magnitude < 0.5f && visibility) HideChar(false);
            }
            else //aiming
            {
                float zoom2 = aimzoom;
                float CamDesp = 0.25f;
                //Camera position
                Vector3 ToPosition;
                Helper.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f);
                Target.transform.position = Helper.transform.position;
                Target.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0f, 0f));

                float myrad = 0.225f;
                if (Physics.SphereCast(Target.transform.position, myrad, -Target.transform.forward, out RaycastHit hit, zoom2 - myrad))
                    ToPosition = hit.point + (hit.normal * myrad) + Helper.transform.right * -CamDesp * 2f;
                else ToPosition = Target.transform.position - (Target.transform.forward * (zoom2 - myrad));

                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, ToPosition, 0.025f);
                Camera.main.transform.LookAt(Target.transform.position + Helper.transform.right * CamDesp);
                if ((Camera.main.transform.position - Target.transform.position).magnitude > 0.25f && !visibility) HideChar(true);
                if ((Camera.main.transform.position - Target.transform.position).magnitude < 0.25f && visibility) HideChar(false);
            }
        }

        // stand 1       
        void SetMoveDir1()
        {
            if (!grounded) dirforw = Model.transform.forward;
            else
            {
                if (!aim)
                {
                    //   hit   hit1
                    Physics.Raycast(trans.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 0.25f);
                    Physics.Raycast(trans.position + Vector3.up * (0.18f * speed + 0.25f) + Model.transform.forward * (0.18f * speed + 0.125f), Vector3.down * (0.18f * speed + 0.3f), out RaycastHit hit1);
                    if (Vector3.Angle(hit.normal, hit1.normal) > 90f) hit1 = hit;
                    dirforw = Vector3.Slerp(dirforw, -Vector3.Cross(hit.normal + hit1.normal, Model.transform.right), turnspeed * Time.deltaTime).normalized;

                    angleforward = Vector3.SignedAngle(Vector3.ProjectOnPlane(dirforw, Vector3.up), dirforw, Model.transform.right);

                    angleright = Mathf.Lerp(angleright, Vector3.SignedAngle(Model.transform.forward, InputMoveDir, Vector3.up) * 0.125f, 12f * Time.deltaTime);
                    if (Mathf.Abs(angleright) < 0.05f) angleright = 0f;
                    anim.SetFloat("tilt", angleright);
                    anim.SetFloat("angle", angleforward);
                    if (!grounded) dirforw = Model.transform.forward;
                    divergence = Mathf.Abs(Vector3.SignedAngle(Model.transform.forward, InputMoveDir, Vector3.up));
                    diverF = 1f - (divergence / 180f);
                }
                else // aim
                {
                    RaycastHit hit; RaycastHit hit1;
                    Physics.Raycast(trans.position + new Vector3(0f, 0.2f, 0f), Vector3.down * 0.25f, out hit);
                    Physics.Raycast(trans.position + new Vector3(0f, 0.2f, 0f) + dirforw * 0.18f * speed, Vector3.down * 0.25f, out hit1);
                    dirforw = Vector3.Slerp(dirforw, -Vector3.Cross(hit.normal + hit1.normal, (Vector3.Cross(InputMoveDir, -Vector3.up))), 18f * Time.deltaTime).normalized;
                    angleforward = Vector3.SignedAngle(Vector3.ProjectOnPlane(dirforw, Vector3.up), dirforw, Model.transform.right);
                    anim.SetFloat("angle", angleforward);

                    divergence = Vector3.SignedAngle(Helper.transform.forward, Vector3.ProjectOnPlane(dirforw, Vector3.up), Vector3.up);
                    anim.SetFloat("tilt", Input.GetAxis("Horizontal") * Aspeed);
                    if (!grounded) dirforw = Model.transform.forward;                    
                }
            }
        }
        void MoveChar1()
        {
            if (!aim)
            {
                if (grounded)
                {
                    if (InputMoveDir.magnitude > 0f)
                    {
                        Quaternion qAUX = Quaternion.LookRotation(InputMoveDir);
                        Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, qAUX, turnspeed * Time.deltaTime);// * Aspeed);
                    }
                    if (!moving && blocked == 1f)
                    {
                        if (Aspeed > 2f)
                        {
                            anim.CrossFade("brake", 0.1f);
                            tospeed = 0f;
                            toAspeed = 0f;
                            Aspeed = 0f;
                            anim.SetFloat("Aspeed", 0f);
                            active = false;
                            Invoke("DelayActive", 0.5f);
                        }
                        else if (Aspeed > 1f)
                        {
                            anim.CrossFade("runOUT", 0.05f);
                            tospeed = 0f;
                            toAspeed = 0f;
                            Aspeed = 0f;
                            speed *= 0.35f;
                            anim.SetFloat("Aspeed", 0f);
                            active = false;
                            Invoke("DelayActive", 0.5f);
                        }
                        else if (Aspeed < 0.2f) { Aspeed = 0f; express = 0f; StopChar(); }
                    }
                    rigid.velocity = dirforw * speed * blocked * diverF;
                }                
            }
            else // aiming
            {
                if (Aspeed < 0.2f) //turn anim
                {
                    float tempdivergence = Vector3.SignedAngle(Helper.transform.forward, Vector3.ProjectOnPlane(Model.transform.forward, Vector3.up), Vector3.up);
                    anim.SetFloat("turn", tempdivergence);
                }
                else anim.SetFloat("turn", 0f);
                Quaternion qAUX = Quaternion.LookRotation(Helper.transform.forward);
                if (grounded)
                {
                    if (Mathf.Abs(VInput) + Mathf.Abs(HInput) < 0.1f)
                        if (Mathf.Abs(Aspeed) + Mathf.Abs(angleright) < 0.2f) { speed = 0f; Aspeed = 0f; express = 0f; }                    
                    Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, qAUX, turnspeed * Time.deltaTime);
                    rigid.velocity = dirforw * speed * blocked;
                }                
            }
        }
        void GetInput1()
        {
            if (canaim)  // aim
            {
                if (Input.GetMouseButtonDown(1))
                {
                    aimONOFF(true);
                }
                else if (Input.GetMouseButtonUp(1)) // !aim
                {
                    aimONOFF(false);
                }
            }
            Debug.DrawRay(trans.position + Vector3.up * 0.12f, InputMoveDir, Color.magenta);
            Debug.DrawRay(trans.position + Vector3.up * 0.12f, dirforw, Color.green);
            //idles            
            if (grounded && !floating && speed == 0) idletime += Time.deltaTime;
            if (idles == 0 && idletime > 3f)
            {
                anim.SetLayerWeight(LayerIdles, 1f);
                idles = Random.Range(1, 7);
                idletime = 0f;
                anim.SetInteger("idles", 0);
            }
            if (Input.anyKey)
            {
                anim.SetLayerWeight(LayerIdles, 0f);
                idles = 0;
                idletime = 0f;
                anim.SetInteger("idles", 0);
            }
            if (idles != 0)
            {
                if (idletime > 1f)
                {
                    anim.SetInteger("idles", Random.Range(1, 7));
                    idletime = 0f;
                }
            }

            CheckFront();

            //walk run sprint
            if (active)
            {
                if (Input.GetButtonDown("Jump") && !floating && grounded) StartCoroutine("Jump");                                        
                
                VInput = Input.GetAxis("Vertical");
                HInput = Input.GetAxis("Horizontal");
                moving = false;
                if (Input.GetKey("a") || Input.GetKey("d") || Input.GetKey("s") || Input.GetKey("w")) moving = true;

                if (moving) // walk run
                {
                    InputMoveDir = (Helper.transform.forward * VInput + Helper.transform.right * HInput).normalized;
                    if (Input.GetKey("left shift"))
                    {
                        if (active)
                        {
                            tospeed = runspeed + express * (sprintspeed - runspeed);
                            toAspeed = 2f + express;
                        }
                    }
                    else
                    {
                        if (active)
                        {
                            tospeed = walkspeed;
                            toAspeed = 1f;
                        }
                    }
                    if (Input.GetKeyDown("left shift") && Aspeed > 1.5f) express = 1f;
                    if (Input.GetKeyUp("left shift")) express = 0f;
                }
                else
                {
                    tospeed = 0f;
                    toAspeed = 0f;
                    if (speed < 0.3f) speed = 0f;
                    if (Aspeed < 0.3f) Aspeed = 0f;
                    express = 0f;
                }
                if (!floating)
                {
                    speed = Mathf.Lerp(speed, tospeed, walkaceleration * Time.deltaTime);
                    Aspeed = Mathf.Lerp(Aspeed, toAspeed, walkaceleration * Time.deltaTime);
                    if (!aim) anim.SetFloat("Aspeed", Aspeed * blocked);
                    else anim.SetFloat("Aspeed", VInput * Aspeed);
                }
            }
            //sitdown
            if (Input.GetKeyDown("e") && grounded)
            {
                int foundone = 0;
                Collider[] hitColliders = Physics.OverlapSphere(trans.position + Vector3.up * tall * 0.5f, 0.35f);
                Collider tempchair = hitColliders[0];

                foreach (var eachCollider in hitColliders)
                {
                    if (eachCollider.transform.tag == "Chair")
                    {
                        foundone = 1;
                        if ((eachCollider.transform.position - trans.position).magnitude < (tempchair.transform.position - trans.position).magnitude)
                            tempchair = eachCollider;
                    }
                    if (eachCollider.transform.tag == "BedR")
                    {
                        foundone = 2;
                        if ((eachCollider.transform.position - trans.position).magnitude < (tempchair.transform.position - trans.position).magnitude)
                            tempchair = eachCollider;
                    }
                    if (eachCollider.transform.tag == "BedL")
                    {
                        foundone = 3;
                        if ((eachCollider.transform.position - trans.position).magnitude < (tempchair.transform.position - trans.position).magnitude)
                            tempchair = eachCollider;
                    }
                    if (eachCollider.transform.tag == "LieDown")
                    {
                        foundone = 4;
                        if ((eachCollider.transform.position - trans.position).magnitude < (tempchair.transform.position - trans.position).magnitude)
                            tempchair = eachCollider;
                    }
                }
                if (foundone != 0) aimONOFF(false);
                if (foundone == 1) //chair
                {
                    StopChar();
                    StartCoroutine("ChairSitDown", tempchair.transform.parent.transform);
                    InputMoveDir = tempchair.transform.parent.transform.forward;
                    dirforw = InputMoveDir;
                    zoomup = 1f;
                }
                if (foundone == 2) //bedR
                {
                    StopChar();
                    StartCoroutine("GoToBedR", tempchair.transform);
                    anim.SetBool("busy", true);
                    InputMoveDir = tempchair.transform.forward;
                    dirforw = InputMoveDir;
                    stand = 100;
                    active = false;
                    zoomup = 1f;
                    floating = true;
                }
                if (foundone == 3) // bedL
                {
                    StopChar();
                    StartCoroutine("GoToBedL", tempchair.transform);
                    anim.SetBool("busy", true);
                    InputMoveDir = tempchair.transform.forward;
                    dirforw = InputMoveDir;
                    stand = 100;
                    active = false;
                    zoomup = 1f;
                    floating = true;
                }
                if (foundone == 4) //beach towel
                {
                    StopChar();
                    anim.SetBool("busy", true);
                    StartCoroutine("LieDown", tempchair.transform);
                }
                System.Array.Clear(hitColliders, 0, hitColliders.Length);
            }
        }
        // slide 3         
        void SetMoveDir3()
        {
            if (Physics.Raycast(trans.position + Vector3.up * tall * 0.5f + dirforw * 0.0125f, -Vector3.up, out RaycastHit hit, tall * 0.6f))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > 0.5f)
                    dirforw = Vector3.Slerp(dirforw, Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal), hit.normal), 50f * Time.deltaTime).normalized;
                else
                    dirforw = Vector3.ProjectOnPlane(dirforw, Vector3.up);

                float fAUX = Vector3.Angle(dorsal, Vector3.up);
                angleforward = Vector3.SignedAngle(Vector3.ProjectOnPlane(dirforw, Vector3.up), dirforw, -Model.transform.right);

                anim.SetFloat("angle", Mathf.Lerp(anim.GetFloat("angle"), angleforward, 10f * Time.deltaTime));

            }
            else
            {
                stand = 1;
                anim.SetBool("busy", false);
                anim.SetBool("grounded", false);
                grounded = false;
                InputMoveDir = Vector3.ProjectOnPlane(dirforw, Vector3.up);
                floating = true;
                Invoke("DelayFloat", 0.125f);
                active = false;
                Invoke("DelayActive", 0.125f);
            }
        }
        void MoveChar3()
        {
            Quaternion qAUX = Quaternion.LookRotation(Vector3.ProjectOnPlane(dirforw, Vector3.up));
            Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, qAUX, 5f * Time.deltaTime);
            if (angleforward < -25f)
            {
                rigid.velocity = dirforw * speed;
                speed += 12f * Time.deltaTime;
            }
            else
            {
                rigid.velocity = dirforw * speed;
                if (speed > 0f) speed -= 12f * Time.deltaTime;
                CheckFront();
            }

            Physics.Raycast(trans.position + Vector3.up * tall * 0.5f, Vector3.down, out RaycastHit hit, tall * 0.6f);
            Vector3 targetposition = trans.position;
            targetposition.y = hit.point.y;
            trans.position = Vector3.Lerp(trans.position, targetposition, 20f * Time.deltaTime);

            if (speed < 1f)
            {
                stand = 1;
                anim.SetBool("busy", false);
                anim.SetFloat("Aspeed", 0f);
                dirforw = Vector3.ProjectOnPlane(dirforw, Vector3.up);
                InputMoveDir = dirforw;
                anim.SetBool("grounded", true);
                grounded = true;
            }
        }
        void GetInput3()
        {
            if (canaim)  // aim
            {
                if (Input.GetMouseButtonDown(1))
                {
                    aimONOFF(true);
                }
                else if (Input.GetMouseButtonUp(1)) // !aim
                {
                    aimONOFF(false);
                }
            }
            if (Input.GetButtonDown("Jump"))   // jump
            {
                stand = 1;
                active = false;
                floating = true;
                Invoke("DelayActive", 0.25f);
                Invoke("DelayFloat", 1f);
                StartCoroutine("Jump");
            }
        }
        // sitdown 5
        void GetInput5()
        {
            //idles            
            idletime += Time.deltaTime;
            if (idles == 0 && idletime > 3f)
            {
                anim.SetLayerWeight(LayerIdles, 1f);
                idles = Random.Range(1, 7);
                idletime = 0f;
                anim.SetInteger("idles", 0);
            }
            if (idles != 0)
            {
                if (idletime > 1f)
                {
                    anim.SetInteger("idles", Random.Range(1, 7));
                    idletime = 0f;
                }
            }
            if (Input.anyKeyDown && active)
            {
                StartCoroutine("ChairStandUp");
            }
        }
        // bedR 6
        void GetInput6()
        {
            //idles            
            idletime += Time.deltaTime;
            if (idles == 0 && idletime > 3f)
            {
                anim.SetLayerWeight(LayerIdles, 1f);
                idles = Random.Range(1, 7);
                idletime = 0f;
                anim.SetInteger("idles", 0);
            }
            if (idles != 0)
            {
                if (idletime > 1f)
                {
                    anim.SetInteger("idles", Random.Range(1, 7));
                    idletime = 0f;
                }
            }
            if (Input.anyKeyDown && active)
            {
                StartCoroutine("WakeUpR");
            }
        }
        // bedL 7
        void GetInput7()
        {
            //idles            
            idletime += Time.deltaTime;
            if (idles == 0 && idletime > 3f)
            {
                anim.SetLayerWeight(LayerIdles, 1f);
                idles = Random.Range(1, 7);
                idletime = 0f;
                anim.SetInteger("idles", 0);
            }
            if (idles != 0)
            {
                if (idletime > 1f)
                {
                    anim.SetInteger("idles", Random.Range(1, 7));
                    idletime = 0f;
                }
            }
            if (Input.anyKeyDown && active)
            {
                StartCoroutine("WakeUpL");
            }
        }
        // beachtowel 8
        void GetInput8()
        {
            if (Input.anyKeyDown && active)
            {
                anim.SetLayerWeight(LayerIdles, 0f);
                anim.SetBool("busy", false);
                newstand = 1;
                Invoke("DelayStand", 1.2f);
            }
        }
        //CHECKS      
        void CheckGround()
        {
            bool oldcheck = grounded;
            float deltaG = 0.125f;
            if (!grounded) deltaG = 0.125f;
            if (Physics.SphereCast(trans.position + Vector3.up * tall * 0.5f, 0.05f, -Vector3.up, out RaycastHit Sphhit, tall * 0.5f + deltaG))
            {
                // ground detected
                if (Vector3.Angle(Sphhit.normal, Vector3.up) < 45f)
                    grounded = true;
                else
                {
                    if (!stairs) grounded = false;
                    dirfall = Vector3.Cross(Vector3.Cross(Vector3.up, Sphhit.normal), Sphhit.normal).normalized;
                }
                //stars & slopes
                Vector3 CheckDir = Model.transform.forward;
                if (aim) CheckDir = InputMoveDir;

                if (Physics.Raycast(trans.position + Vector3.up * tall * 0.5f + CheckDir * Aspeed * 0.1f, -Vector3.up, out RaycastHit hit, (tall * 0.5f) + 1f))
                {
                    if (hit.transform.CompareTag("Stair")) { anim.SetFloat("stairs", 1f); stairs = true; }
                    else { anim.SetFloat("stairs", 0f); stairs = false; }
                    if (hit.transform.CompareTag("Slope")) slope = true;
                    else { slope = false; }

                    if (slope)
                    {
                        dorsal = hit.normal;
                        GoSlide();
                        dirfall = Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal), hit.normal).normalized;
                    }
                    //adapt to de ground
                    Vector3 targetposition = trans.position;
                    targetposition.y = Sphhit.point.y;
                    trans.position = Vector3.Lerp(trans.position, targetposition, Time.deltaTime * 20f);
                }
            }
            else // no ground detected
            {
                grounded = false;
                dirfall = -Vector3.up;
            }
            if (grounded) rigid.drag = 1;
            else rigid.drag = 0;

            // land hard
            if (!oldcheck && grounded && grtime > 0.75f)
            {
                if (Aspeed <= 0.5f)
                {
                    StopChar();
                    anim.Play("landhard");
                    anim.Play("landhard",2);
                    active = false;
                    Invoke("DelayActive", 1f);
                }
                else
                {
                    anim.Play("roll");
                    anim.Play("roll",2);
                }
            }

        }
        void CheckFront()
        {
            // wall ahead
            Vector3 CheckDir = Model.transform.forward;
            if (aim) CheckDir = InputMoveDir;
            if (Physics.SphereCast(trans.position + Model.transform.up * tall * 0.5f, 0.15f, CheckDir, out RaycastHit hitfront, (0.125f * Aspeed) + 0.1f))
            {
                blocked = 0f;
                speed = 0f;
                Aspeed = 0f;
                express = 0f;
            }
            else blocked = 1;
        }
        //TOOLS
        void StopChar()
        {
            rigid.velocity = Vector3.zero;
            Aspeed = 0f;
            speed = 0f;
            anim.SetFloat("Aspeed", 0f);
            tospeed = 0f;
            toAspeed = 0f;
            express = 0;
        }
        void HideChar(bool ONOFF)
        {
            foreach (Transform child in Model.transform)
            {
                if (child.gameObject.activeSelf && child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) child.GetComponent<SkinnedMeshRenderer>().enabled = ONOFF;
                if (child.gameObject.activeSelf && child.gameObject.GetComponent<Renderer>() != null) child.GetComponent<Renderer>().enabled = ONOFF;
                if (GlassesONOFF) Glasses.gameObject.SetActive(ONOFF);
            }
            visibility = ONOFF;
        }
        void Gravity()
        {
            if (!grounded)
            {
                if ((rigid.velocity.y * Vector3.up).magnitude > 3f)
                    grtime += Time.deltaTime;
                rigid.velocity += dirfall * 18f * Time.deltaTime;
            }
            else
            {
                grtime = 0f;
                anim.SetBool("grounded", true);
            }
            if (grtime > 0.15f)
            {
                anim.SetBool("grounded", false);
            }
        }

        void aimONOFF(bool ONOFF)
        {
            if (ONOFF)
            {
                aim = true;
                Camera.main.fieldOfView = 30f;
                InputMoveDir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                anim.SetLayerWeight(LayerAim, 1f);
                MyCross.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                aim = false;
                Camera.main.fieldOfView = 60f;
                anim.SetLayerWeight(LayerAim, 0f);
                MyCross.GetComponent<Canvas>().enabled = false;
            }
        }
        void SelectChar()
        {
            bool randgender = false;
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            if (!Male && !Female) randgender = true;
            if (Male && Female) randgender = true;
            if (Male && !Female)
            {
                Character = Instantiate(Males[Random.Range(0, 4)], transform);
                anim = Character.GetComponent<Animator>();
                anim.runtimeAnimatorController = MaleControllerAnimations;
            }
            if (Female && !Male)
            {
                Character = Instantiate(Females[Random.Range(0, 4)], transform);
                anim = Character.GetComponent<Animator>();
                anim.runtimeAnimatorController = FemaleControllerAnimations;
            }
            if (randgender)
            {
                int AUX = Random.Range(0, 8);
                if (AUX < 4)
                {
                    Character = Instantiate(Males[AUX], transform);
                    anim = Character.GetComponent<Animator>();
                    anim.runtimeAnimatorController = MaleControllerAnimations;
                }
                else
                {
                    Character = Instantiate(Females[AUX - 4], transform);
                    anim = Character.GetComponent<Animator>();
                    anim.runtimeAnimatorController = FemaleControllerAnimations;
                }
            }
            if (Character.GetComponent<TPMalePrefabMaker>() != null)
            {
                Character.GetComponent<TPMalePrefabMaker>().Getready();
                Character.GetComponent<TPMalePrefabMaker>().Randomize();
                if (Elder) Character.GetComponent<TPMalePrefabMaker>().ElderOn();
            }
            if (Character.GetComponent<TPFemalePrefabMaker>() != null)
            {
                Character.GetComponent<TPFemalePrefabMaker>().Getready();
                Character.GetComponent<TPFemalePrefabMaker>().Randomize();
                if (Elder) Character.GetComponent<TPFemalePrefabMaker>().ElderOn();
            }
            if (Elder) anim.runtimeAnimatorController = ElderControllerAnimations;
            if (Character.TryGetComponent(out Playanimation temp))
                temp.enabled = false;
            if (Character.TryGetComponent(out CapsuleCollider temp2))
                temp2.enabled = false;

            Model = Character;
        }
        void RandomizeChar()
        {
            if (Character.GetComponent<TPMalePrefabMaker>() != null)
            {
                Character.GetComponent<TPMalePrefabMaker>().Getready();
                Character.GetComponent<TPMalePrefabMaker>().Randomize();
            }
            if (Character.GetComponent<TPFemalePrefabMaker>() != null)
            {
                Character.GetComponent<TPFemalePrefabMaker>().Getready();
                Character.GetComponent<TPFemalePrefabMaker>().Randomize();
            }
            GlassesONOFF = Glasses.gameObject.activeSelf;
        }
        void DelayActive()
        {
            active = true;
        }
        void DelayFloat()
        {
            floating = false;
        }
        void DelayStand()
        {
            stand = newstand;
        }
        void GoSlide()
        {
            stand = 3;
            anim.SetBool("busy", true);
            anim.CrossFade("slide", 0.1f);
            anim.Play("slide", 2);
            speed = 1.75f;
        }
       
        IEnumerator Jump()
        {
            active = false;
            floating = true;
            Invoke("DelayActive", 0.35f);
            Invoke("DelayFloat", 0.4f);
            grounded = false;
            anim.SetBool("grounded", false);
            if (Aspeed < 0.25)
            {
                anim.Play("jump");
                anim.Play("jump",2);
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                anim.Play("runjump");
                anim.Play("runjump",2);
                yield return new WaitForSeconds(0.1f);
            }
            toAspeed = 0f;
            speed *= 0.5f;
            if (Physics.SphereCast(trans.position + Vector3.up * 0.5f, 0.175f, -Vector3.up, out RaycastHit hit, 0.6f))
                rigid.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            else anim.Play("freefall");
        }
        IEnumerator ChairSitDown(Transform targetT)
        {
            stand = 100;
            CharCol.enabled = false;
            float dist = (trans.position - targetT.position).magnitude;
            while (dist > 0.0125f)
            {
                trans.position = Vector3.MoveTowards(trans.position, targetT.position, 1.25f * Time.deltaTime);
                dist = (trans.position - targetT.position).magnitude;
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, targetT.rotation, 12f * Time.deltaTime);
                yield return null;
            }
            Model.transform.rotation = targetT.rotation;
            anim.applyRootMotion = true;
            //sit down
            anim.Play("sitdown");
            yield return new WaitForSeconds(0.75f);
            //move chair
            targetT.transform.parent = Model.transform;
            anim.Play("sitdownmovechairIN");
            yield return new WaitForSeconds(1.25f);
            //idle
            anim.Play("sitidle1", 1);
            float temp = 0f;
            while (temp < 1f)
            {
                temp += 2f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            anim.SetLayerWeight(LayerIdles, 1f);
            active = true;
            stand = 5;
        }
        IEnumerator ChairStandUp()
        {
            stand = 100;
            float temp = 1f;
            while (temp > 0f)
            {
                temp -= 5f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            anim.SetLayerWeight(LayerIdles, 0f);
            Transform Chair = Model.transform.Find("Chair");
            anim.SetLayerWeight(LayerIdles, 0f);
            anim.Play("sitdownmovechairOUT");
            yield return new WaitForSeconds(2.5f);
            Chair.transform.parent = null;
            anim.applyRootMotion = false;
            anim.Play("idle", 1);
            active = true;
            stand = 1;
            CharCol.enabled = true;
        }
        IEnumerator GoToBedR(Transform targetT)
        {
            stand = 100;
            CharCol.enabled = false;
            float dist = (trans.position - targetT.position).magnitude;
            while (dist > 0.005f)
            {
                trans.position = Vector3.MoveTowards(trans.position, targetT.position, 1.25f * Time.deltaTime);
                dist = (trans.position - targetT.position).magnitude;
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, targetT.rotation, 12f * Time.deltaTime);
                yield return null;
            }
            Model.transform.rotation = targetT.rotation;
            anim.Play("sitdown");
            yield return new WaitForSeconds(0.75f);
            anim.applyRootMotion = true;
            anim.Play("sitdownliedownR");
            yield return new WaitForSeconds(1.75f);
            anim.Play("idlesleep1", 1);
            float temp = 0f;
            while (temp < 1f)
            {
                temp += 2f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            idletime = 0f;
            active = true;
            stand = 6;
        }
        IEnumerator WakeUpR()
        {
            stand = 100;
            float temp = 1f;
            while (temp > 0f)
            {
                temp -= 5f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            anim.SetLayerWeight(LayerIdles, 0f);
            anim.Play("idle", 1);
            anim.Play("wakeupR");
            yield return new WaitForSeconds(2.25f);
            anim.applyRootMotion = false;
            active = true;
            floating = false;
            CharCol.enabled = true;
            stand = 1;
            Model.transform.position = trans.position;
        }
        IEnumerator GoToBedL(Transform targetT)
        {
            stand = 100;
            CharCol.enabled = false;
            float dist = (trans.position - targetT.position).magnitude;
            while (dist > 0.005f)
            {
                trans.position = Vector3.MoveTowards(trans.position, targetT.position, 1.25f * Time.deltaTime);
                dist = (trans.position - targetT.position).magnitude;
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, targetT.rotation, 12f * Time.deltaTime);
                yield return null;
            }
            Model.transform.rotation = targetT.rotation;
            anim.Play("sitdown");
            yield return new WaitForSeconds(0.75f);
            anim.Play("sitdownliedownL");
            anim.applyRootMotion = true;
            yield return new WaitForSeconds(1.75f);
            anim.Play("idlesleep1", 1);
            float temp = 0f;
            while (temp < 1f)
            {
                temp += 2f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            idletime = 0f;
            active = true;
            stand = 7;
        }
        IEnumerator WakeUpL()
        {
            stand = 100;
            float temp = 1f;
            while (temp > 0f)
            {
                temp -= 5f * Time.deltaTime;
                anim.SetLayerWeight(LayerIdles, temp);
                yield return null;
            }
            anim.SetLayerWeight(LayerIdles, 0f);
            anim.Play("idle", 1);
            anim.Play("wakeupL");
            yield return new WaitForSeconds(2.25f);
            anim.applyRootMotion = false;
            active = true;
            floating = false;
            CharCol.enabled = true;
            stand = 1;
            Model.transform.position = trans.position;
        }
        IEnumerator LieDown(Transform targetT)
        {
            stand = 100;
            float dist = (trans.position - targetT.position).magnitude;
            anim.SetFloat("Aspeed", 0f);
            anim.SetFloat("tilt", 0f);
            anim.SetLayerWeight(LayerAim, 1f);
            anim.SetFloat("turn", 0f);
            while (dist > 0.0125f)
            {
                float tempangleX = Vector3.Angle(Model.transform.forward, trans.position - targetT.position);
                float tempangleY = Vector3.Angle(Model.transform.right, trans.position - targetT.position);
                float tempangleX2 = (tempangleX - 90f) / 90f;
                float tempangleY2 = (tempangleY - 90f) / 90f;
                Mathf.Clamp(tempangleX2, -1f, 1f);
                Mathf.Clamp(tempangleY2, -1f, 1f);
                anim.SetFloat("Aspeed", tempangleX2);
                anim.SetFloat("tilt", tempangleY2);

                trans.position = Vector3.MoveTowards(trans.position, targetT.position, 1.5f * Time.deltaTime);
                dist = (trans.position - targetT.position).magnitude;
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, targetT.rotation, 4f * Time.deltaTime);
                yield return null;
            }
            float tempangle = Vector3.Angle(Model.transform.forward, targetT.forward);
            while (tempangle > 2f)
            {
                if (Vector3.SignedAngle(Model.transform.forward, targetT.forward, Vector3.up) > 0.25f)
                {
                    anim.SetFloat("turn", -10f);
                    Model.transform.Rotate(Vector3.up * 300f * Time.deltaTime);
                }
                if (Vector3.SignedAngle(Model.transform.forward, targetT.forward, Vector3.up) < -0.25f)
                {
                    anim.SetFloat("turn", 10f);
                    Model.transform.Rotate(Vector3.up * -300f * Time.deltaTime);
                }
                tempangle = Vector3.Angle(Model.transform.forward, targetT.forward);
                yield return null;
            }
            anim.SetFloat("turn", 0f);
            anim.SetLayerWeight(LayerAim, 0f);
            anim.Play("liedownIN", 0); 
            yield return new WaitForSeconds (2f);
            anim.Play("sunbath");
            anim.SetBool("busy", true);
            stand = 8;
            InputMoveDir = targetT.transform.forward;
            dirforw = InputMoveDir;
        }
    }
}





