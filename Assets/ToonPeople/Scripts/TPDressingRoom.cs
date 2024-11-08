using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToonPeople
{
    public class TPDressingRoom : MonoBehaviour
    {
        public Camera Mcam;
        bool MovilCam;
        public GameObject[] Boys;
        public GameObject[] Girls;
        GameObject[] ActiveChars;
        public GameObject Top;
        public GameObject Head;
        public GameObject Chest;
        public GameObject Legs;
        public GameObject Feet;
        public GameObject TopGirl;
        public GameObject HeadGirl;
        public GameObject ChestGirl;
        public GameObject LegsGirl;
        public GameObject FeetGirl;
        public GameObject GirlLegs;
        public GameObject MalePanel;
        public GameObject FemalePanel;
        public GameObject ZoomOUTPanel;
        GameObject Character;
        public RuntimeAnimatorController DRMaleAnimations;
        public RuntimeAnimatorController DRFemaleAnimations;
        public RuntimeAnimatorController DRElderAnimations;
        Animator anim;
        public Transform[] campositions;
        int Cam2Pos;
        bool OnPosition;
        Vector3 Distance;
        Transform CamStartPosition;
        Transform CamEndPosition;
        GameObject[] Panels;
        int CharIndex;
        int gender;
        int panel;
        float[] LookAt;
        Transform Target;
        float idletime;
        float nextidle;
        float ElderCam;
        bool ElderON;

        void Start()
        {
            ElderCam = 0f;
            MovilCam = true;
            Cam2Pos = 5;
            idletime = 0f;
            nextidle = 2f;
            panel = 0;
            Mcam.transform.position = campositions[5].position;
            Mcam.transform.rotation = campositions[5].rotation;
            CharIndex = 0;
            Character = transform.GetChild(1).gameObject;
            if (Character.TryGetComponent(out Playanimation temp))
                temp.enabled = false;
            if (Character.TryGetComponent(out TPMalePrefabMaker temp2))
            {
                temp2.Getready();
                temp2.Randomize();
                MalePanel.SetActive(true);
                FemalePanel.SetActive(false);
            }
            if (Character.TryGetComponent(out TPFemalePrefabMaker temp3))
            {
                temp3.Getready();
                temp3.Randomize();
                MalePanel.SetActive(false);
                FemalePanel.SetActive(true);
            }
            anim = Character.GetComponent<Animator>();
            if (gender == 0) anim.runtimeAnimatorController = DRMaleAnimations;
            else anim.runtimeAnimatorController = DRFemaleAnimations;
            anim.Play("idle");
            OnPosition = true;
            Panels = new GameObject[10] { Top, Head, Chest, Legs, Feet, TopGirl, HeadGirl, ChestGirl, LegsGirl, FeetGirl };
            for (int forAUX = 0; forAUX < Panels.Length - 1; forAUX++)
                Panels[forAUX].SetActive(false);
            ActiveChars = Boys;
            gender = 0;
            Target = transform.GetChild(0);
            LookAt = new float[6] { 1.238f, 1.238f, 0.851f, 0.358f, 0.048f, 0.851f };
            Target.transform.position = transform.position + Vector3.up * LookAt[0];
            CamStartPosition = Mcam.transform;
            CamEndPosition = campositions[5];
        }

        void Update()
        {
            if (!OnPosition) MoveCam();
            idletime += Time.deltaTime;
            if (idletime > nextidle)
            {
                if (Cam2Pos == 5 || !MovilCam) anim.SetInteger("idles", Random.Range(0, 24));
                else anim.SetInteger("idles", 0);
                nextidle = Random.Range(2f, 4f);
                idletime = 0f;
            }
            transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * -90f);
            if (Input.GetKey("s")) transform.rotation = Quaternion.Euler(Vector3.up);
            if (Input.GetKey("w")) { transform.rotation = Quaternion.Euler(Vector3.up); transform.Rotate(Vector3.up * 180f); }
        }

        //BOYS
        //Hairs & hats
        public void SetHair(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Sethair(index);
            if (Coin(10)) anim.Play("hair" + Random.Range(0, 2));
        }        
        public void NextHairColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexthaircolor(0);
        }
        public void PrevHairColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexthaircolor(1);
        }
        public void NextHatColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexthatcolor(0);
        }
        public void PrevHatColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexthatcolor(1);
            if (Coin(10)) anim.Play("hair");
        }
        //Head
        public void NextSkinColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextskincolor(0);
            if (Coin(8)) anim.Play("skin" + Random.Range(0, 2));
        }
        public void PrevSkinColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextskincolor(1);
            if (Coin(8)) anim.Play("skin" + Random.Range(0, 2));
        }
        public void NextEyesColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexteyescolor(0);
            if (Coin(8)) anim.Play("eyes" + Random.Range(0, 2));
        }
        public void PrevEyesColor()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nexteyescolor(1);
            if (Coin(8)) anim.Play("eyes" + Random.Range(0, 2));
        }
        public void NextGlasses()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextglasses(0);
        }
        public void PrevGlasses()
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextglasses(1);
        }
        public void GlassesONOFF()
        {
            Character.GetComponent<TPMalePrefabMaker>().GlassesOn();
            if (Character.GetComponent<TPMalePrefabMaker>().glassesactive)  anim.Play("glasses" + Random.Range(0, 2));
        }
        public void BeardONOFF(int ONOFF)
        {
            if (ONOFF == 0) Character.GetComponent<TPMalePrefabMaker>().BeardOFF();
            else if (ONOFF == 1) Character.GetComponent<TPMalePrefabMaker>().BeardON();
        }
        public void Beard(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().BeardPart(index);
            if (Coin(8)) anim.Play("beard" + Random.Range(0, 2));
        }
        //Chest
        public void SetChest(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Setchest(index);
            if (Coin(8)) anim.Play("chest" + Random.Range(0, 2));
        }
        public void NextChestColor(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextchestcolor(index);
        }
        public void NextJacketColor(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextjacketcolor(index);
        }
        public void SetTie(int index)
        {
            if (gender == 0)
            {
                if (Character.GetComponent<TPMalePrefabMaker>().tieactive)
                {
                    Character.GetComponent<TPMalePrefabMaker>().Settie(index);
                    if (Coin(8) && Character.GetComponent<TPMalePrefabMaker>().tieactivecolor)
                        anim.Play("tie");
                }
            }
            else
            {
                if (Character.GetComponent<TPFemalePrefabMaker>().tieactive)
                {
                    Character.GetComponent<TPFemalePrefabMaker>().Settie(index);
                    if (Coin(8) && Character.GetComponent<TPFemalePrefabMaker>().tieactivecolor)
                        anim.Play("tie");
                }
            }
        }
        public void NextTieColor(int index)
        {
            if (gender == 0) Character.GetComponent<TPMalePrefabMaker>().Nexttiecolor(index);
            else Character.GetComponent<TPFemalePrefabMaker>().Nexttiecolor(index);
        }
        //Legs
        public void SetLegs(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Setlegs(index);
            if (Coin(8)) anim.Play("legs" + Random.Range(0, 2));
        }
        public void NextLegsColor(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextlegscolor(index);
        }
        //Feet
        public void SetFeet(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Setfeet(index);
            if (Coin(8)) anim.Play("feet" + Random.Range(0, 2));
        }
        public void NextFeetColor(int index)
        {
            Character.GetComponent<TPMalePrefabMaker>().Nextfeetcolor(index);
        }

        //GIRLS
        //Hairs & hats
        public void SetHairGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Sethair(index);
            if (Coin(10)) anim.Play("hair" + Random.Range(0, 2));
        }
        public void NextHairGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexthair();
            if (Coin(10)) anim.Play("hair");
        }
        public void PrevHairGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Prevhair();
            if (Coin(10)) anim.Play("hair");
        }
        public void NextHairColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexthaircolor(0);
            if (Coin(10)) anim.Play("hair");
        }
        public void PrevHairColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexthaircolor(1);
            if (Coin(10)) anim.Play("hair");
        }
        public void NextHatColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexthatcolor(0);
            if (Coin(10)) anim.Play("hair");
        }
        public void PrevHatColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexthatcolor(1);
            if (Coin(10)) anim.Play("hair");
        }
        //Head
        public void NextSkinColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextskincolor(0);
            if (Coin(8)) anim.Play("skin" + Random.Range(0, 2));
        }
        public void PrevSkinColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextskincolor(1);
            if (Coin(8)) anim.Play("skin" + Random.Range(0, 2));
        }
        public void NextEyesColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexteyescolor(0);
            if (Coin(8)) anim.Play("eyes" + Random.Range(0, 2));
        }
        public void PrevEyesColorGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nexteyescolor(1);
            if (Coin(8)) anim.Play("eyes" + Random.Range(0, 2));
        }  
        public void NextGlassesGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextglasses(0);
            
        }
        public void PrevGlassesGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextglasses(1);
            
        }
        public void GlassesONOFFGirl()
        {
            Character.GetComponent<TPFemalePrefabMaker>().GlassesOn();
            if (Character.GetComponent<TPFemalePrefabMaker>().glassesactive) anim.Play("glasses" + Random.Range(0, 2));
        }
        //Chest
        public void SetChestGirl(int index)
        {
            GirlLegs.SetActive(true);
            Character.GetComponent<TPFemalePrefabMaker>().Setchest(index);
            CheckSkirt();
            if (Coin(8)) anim.Play("chest" + Random.Range(0, 2));
        }
        public void NextChestColorGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextchestcolor(index);
        }
        public void NextJacketColorGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextjacketcolor(index);
        }
        //Legs
        public void SetLegsGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Setlegs(index);
            if (Coin(8)) anim.Play("legs" + Random.Range(0, 2));
        }
        public void NextLegsColorGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextlegscolor(index);
        }
        //Feet
        public void SetFeetGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Setfeet(index);
            if (Coin(8)) anim.Play("feet" + Random.Range(0, 2));
        }
        public void NextFeetColorGirl(int index)
        {
            Character.GetComponent<TPFemalePrefabMaker>().Nextfeetcolor(index);
        }
        public void JacketONOFF(int index)
        {
            if (gender == 0) Character.GetComponent<TPMalePrefabMaker>().Jacketon(index);
            else Character.GetComponent<TPFemalePrefabMaker>().Jacketon(index);
        }
        public void ActivePanel(int index)
        {
            //panelsON = true;
            panel = index;
            int aux = 5 * gender;
            for (int forAUX = 0; forAUX < Panels.Length; forAUX++)
                Panels[forAUX].SetActive(false);
            if (index < Panels.Length)
            {
                Panels[index + aux].SetActive(true);
            }
            CheckSkirt();
            //anim.SetBool("busy", true);
        }
        public void Randomize()
        {
            if (gender == 0) Character.GetComponent<TPMalePrefabMaker>().Randomize();
            else Character.GetComponent<TPFemalePrefabMaker>().Randomize();
            CheckSkirt();
            CamPos(5);
            for (int forAUX = 0; forAUX < Panels.Length; forAUX++)
                Panels[forAUX].SetActive(false);
            if (Coin(8)) anim.Play("randomize" + Random.Range(0,2));
        }
        public void Nude()
        {
            if (gender == 0) Character.GetComponent<TPMalePrefabMaker>().Nude();
            else Character.GetComponent<TPFemalePrefabMaker>().Nude();
        }
        public void CamPos(int index)
        {
            Cam2Pos = index;
            ElderCam = 0f;
            CamStartPosition = Mcam.transform;
            if (MovilCam) CamEndPosition = campositions[Cam2Pos];
            else CamEndPosition = campositions[5];
            Distance = CamEndPosition.position - Mcam.transform.position;
            if (Distance.magnitude > 0.0125f) OnPosition = false;
            Target.transform.position = transform.position + Vector3.up * LookAt[index];
            if (Cam2Pos == 0 || Cam2Pos == 1)
            {
                if (ElderON) ElderCam = -0.1f;
                else ElderCam = 0f;
            }
        }
        public void ChangeGender()
        {
            for (int forAUX = 0; forAUX < Panels.Length; forAUX++)
                Panels[forAUX].SetActive(false);
            //panelsON = false;//if (panelsON) ActivePanel(panel);
            if (gender == 0)
            {
                gender = 1;
                ActiveChars = Girls;
                NextCharacter(0);
                MalePanel.SetActive(false);
                FemalePanel.SetActive(true);
                if (ElderON) Character.GetComponent<TPFemalePrefabMaker>().ElderOn();
            }
            else
            {
                gender = 0;
                ActiveChars = Boys;
                NextCharacter(0);
                MalePanel.SetActive(true);
                FemalePanel.SetActive(false);
                if (ElderON) Character.GetComponent<TPMalePrefabMaker>().ElderOn();
            }
            CamPos(5);
            //CheckHood();
        }
        public void NextCharacter(int next)
        {
            Destroy(Character);
            CharIndex += next;
            if (CharIndex < 0) CharIndex = 3;
            else if (CharIndex > 3) CharIndex = 0;
            Character = Instantiate(ActiveChars[CharIndex], transform);
            if (Character.TryGetComponent(out Playanimation temp))
                temp.enabled = false;
            if (gender == 0)
            {
                Character.GetComponent<TPMalePrefabMaker>().Getready();
                if (ElderON) Character.GetComponent<TPMalePrefabMaker>().ElderOn();
            }
            else
            {
                Character.GetComponent<TPFemalePrefabMaker>().Getready();
                if (ElderON) Character.GetComponent<TPFemalePrefabMaker>().ElderOn();
            }

            anim = Character.GetComponent<Animator>();
            if (gender == 0)
            {
                anim.runtimeAnimatorController = DRMaleAnimations;
                if (ElderON)
                    anim.runtimeAnimatorController = DRElderAnimations;
            }
            else
            {
                anim.runtimeAnimatorController = DRFemaleAnimations;
                if (ElderON)
                    anim.runtimeAnimatorController = DRElderAnimations;
            }
            int coin = Random.Range(0, 7);
            if (coin == 0) anim.Play("salute1");
            else if (coin == 1) anim.Play("salute2");
            else anim.Play("wave");
            if (gender == 1)
            {
                if (!Character.GetComponent<TPFemalePrefabMaker>().legsactive) GirlLegs.SetActive(false);
                else GirlLegs.SetActive(true);
            }
            else GirlLegs.SetActive(true);
            CheckSkirt();
        }
        public void ElderONOFF()
        {
            if (gender == 0)
            {
                if (!Character.GetComponent<TPMalePrefabMaker>().elder)
                {
                    Character.GetComponent<TPMalePrefabMaker>().ElderOn();
                    anim.runtimeAnimatorController = DRElderAnimations;
                    ElderON = true;                    
                }
                else
                {
                    Character.GetComponent<TPMalePrefabMaker>().ElderOff();
                    anim.runtimeAnimatorController = DRMaleAnimations;
                    ElderON = false;
                }
            }
            else
            {
                if (!Character.GetComponent<TPFemalePrefabMaker>().elder)
                {
                    Character.GetComponent<TPFemalePrefabMaker>().ElderOn();
                    anim.runtimeAnimatorController = DRElderAnimations;
                    ElderON = true;
                }
                else
                {
                    Character.GetComponent<TPFemalePrefabMaker>().ElderOff();
                    anim.runtimeAnimatorController = DRFemaleAnimations;
                    ElderON = false;
                }
            }
            OnPosition = false;
            if (Cam2Pos == 0 || Cam2Pos == 1)
            {
                if (ElderON) ElderCam = -0.1f;
                else ElderCam = 0f;
            MoveCam();
            }
        }
        public void Embarrasment()
        {
            anim.CrossFade("embarrassed", 0.025f);
            //anim.SetBool("busy", false);
        }
        public void LockZoomONOFF()
        {
            MovilCam = !MovilCam;
            if (MovilCam) CamEndPosition = campositions[Cam2Pos];
            else CamEndPosition = campositions[5];
            OnPosition = false;
            MoveCam();
        }
        void CheckSkirt()
        {
            if (gender == 1)
            {
                if (!Character.GetComponent<TPFemalePrefabMaker>().legsactive) GirlLegs.SetActive(false);
                else GirlLegs.SetActive(true);
            }
        }
        void MoveCam()
        {
            Distance = CamEndPosition.position + (Vector3.up*ElderCam) - Mcam.transform.position;
            Mcam.transform.position = Vector3.Lerp(CamStartPosition.position, CamEndPosition.position + (Vector3.up * ElderCam), 0.0125f);
            Mcam.transform.rotation = Quaternion.Lerp(CamStartPosition.rotation, CamEndPosition.rotation, 0.0125f);
            if (Distance.magnitude < 0.0125f) OnPosition = true;
        }
        bool Coin(int range)
        {
            if (Random.Range(0, range) < 5)
                return true;
            else return false;

        }
    }
}