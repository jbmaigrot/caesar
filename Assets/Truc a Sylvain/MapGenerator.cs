using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantForMapGen
{
    public const int playerStart = -1;
    public const int wall = -2;
}

public class Case
{
    public int posX;
    public int posY;
    public int roomtype;
}

public class MapGenerator : MonoBehaviour
{
    private Case[] Grille;
    private int lengthX;
    private int lengthY;
    
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)Time.time * 987654321);
        lengthX = (int)Random.Range(125f, 250f);
        lengthY = (int)Random.Range(250f, 500f);
        Grille = new Case[lengthX * lengthY];
        for (int ryan = 0; ryan < lengthX; ryan++)
        {
            for (int reynolds = 0; reynolds < lengthY; reynolds++)
            {
                Grille[ryan + reynolds * lengthX] = new Case();
                Grille[ryan + reynolds * lengthX].posX = ryan;
                Grille[ryan + reynolds * lengthX].posY = reynolds;
                Grille[ryan + reynolds * lengthX].roomtype = 0;
            }

        }
        MakeRoom(1, 1, lengthX-1, 1, lengthY-1, lengthY-1, lengthY-1, 1, 1, 0);
        for(int ryan = 1; ryan <lengthX; ryan++)
        {
            for(int reynolds = 1;reynolds <lengthY; reynolds++)
            {
                
            }
            
        }

    }

    void MakeRoom(int Ax, int Ay, int Bx, int Cy, int Dy, int Ey, int Gy, int Hy, int numero_room, int failed_attempt)
    {
        float dice = Random.value;
        int centerX = Ax;
        int centerY = Ay;
        int parcoursDebutligne = Ax;
        int parcoursFinLigne = Bx;
        float centerDiceValue = 0;
        bool checkb;
        bool checkt;

        int Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb, Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt;

        if (failed_attempt > 9)
        {
            for (int parcoursY = Ay; parcoursY < Ey; parcoursY++)
            {
                for (int parcoursX = parcoursDebutligne; parcoursX < parcoursFinLigne; parcoursX++)
                {
                    Grille[parcoursX + parcoursY * lengthX].roomtype = numero_room;
                }
                if (parcoursY < Hy)
                {
                    parcoursDebutligne--;
                }
                else if (parcoursY >= Gy)
                {
                    parcoursDebutligne++;
                }
                if (parcoursY < Cy)
                {
                    parcoursFinLigne--;
                }
                else if (parcoursY >= Dy)
                {
                    parcoursFinLigne++;
                }
            }
        }
        else
        {
            for (int parcoursY = Ay; parcoursY < Ey; parcoursY++)
            {
                for (int parcoursX = parcoursDebutligne; parcoursX < parcoursFinLigne; parcoursX++)
                {
                    dice = Random.value;
                    if (dice > centerDiceValue)
                    {
                        centerX = parcoursX;
                        centerY = parcoursY;
                        centerDiceValue = dice;
                    }
                }
                if (parcoursY < Hy)
                {
                    parcoursDebutligne--;
                }
                else if (parcoursY >= Gy)
                {
                    parcoursDebutligne++;
                }
                if (parcoursY < Cy)
                {
                    parcoursFinLigne--;
                }
                else if (parcoursY >= Dy)
                {
                    parcoursFinLigne++;
                }
            }

            dice = Random.value;
            //X cut
            if (dice < 0.5)
            {
                Axb = centerX < Ax ? centerX : Ax;
                Ayb = centerX < Ax ? Ay + Ax - centerX : Ay;
                Bxb = centerX < Bx ? centerX : Bx; ;
                Cyb = centerX < Ax ? Ay + Ax - centerX : centerX < Bx ? Ay : Ay + centerX - Bx;
                Dyb = centerX < Ax + Ay + Ey - Hy - Gy ? Gy + centerX + Hy - Ax - Ay : centerX < Bx + Cy + Dy - Ay - Ey ? Ey : Dy + Bx + Cy - Ay - centerX;
                Eyb = centerX < Ax + Ay + Ey - Hy - Gy ? Gy + centerX + Hy - Ax - Ay : Ey;
                Gyb = Gy;
                Hyb = Hy;

                Axt = centerX > Ax ? centerX : Ax;
                Ayt = centerX > Bx ? Ay + centerX - Bx : Ay;
                Bxt = centerX > Bx ? centerX : Bx;
                Cyt = Cy;
                Dyt = Dy;
                Eyt = centerX > Bx + Cy + Dy - Ay - Ey ? Dy + Cy + Bx - Ay - centerX : Ey;
                Gyt = Dyb;
                Hyt = Cyb;

                checkb = CheckRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb);
                checkt = CheckRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt);

                if (checkb)
                {
                    if (checkt)
                    {
                        MakeRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb, numero_room * 2, 0);
                        MakeRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt, numero_room * 2 + 1, 0);
                    }
                    else
                    {
                        MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                    }
                }
                else
                {
                    MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                }
            }
            //Y cut
            else if (dice < 1)
            {
                Axb = Ax;
                Ayb = Ay;
                Bxb = Bx;
                Cyb = Cy < centerY ? Cy : centerY;
                Dyb = Dy < centerY ? Dy : centerY;
                Eyb = centerY;
                Gyb = Gy < centerY ? Gy : centerY;
                Hyb = Hy < centerY ? Hy : centerY;

                Axt = centerY < Hy ? Ax - centerY + Ay : centerY < Gy ? Ax - Hy + Ay : Ax - Hy + Ay + centerY - Gy;
                Ayt = centerY;
                Bxt = centerY < Cy ? Bx + centerY - Ay : centerY < Dy ? Bx + Cy - Ay : Bx + Cy + Ay - centerY + Dy;
                Cyt = Cy > centerY ? Cy : centerY;
                Dyt = Dy > centerY ? Dy : centerY;
                Eyt = Ey;
                Gyt = Gy > centerY ? Gy : centerY;
                Hyt = Hy > centerY ? Hy : centerY;

                checkb = CheckRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb);
                checkt = CheckRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt);

                if (checkb)
                {
                    if (checkt)
                    {
                        MakeRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb, numero_room * 2, 0);
                        MakeRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt, numero_room * 2 + 1, 0);
                    }
                    else
                    {
                        MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                    }
                }
                else
                {
                    MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                }
            }
            //positive diagonal cut
            /*else if (dice < 0.8)
            {
                int interHLX;
                int interHLY;
                int interDRX;
                int interDRY;

                int x = centerX;
                int y = centerY;

                while (y>Ay && x>Ax-Ay+Hy && x+y >Ax+Ay)
                {
                    x--;
                    y--;                    
                }
                interHLX = x;
                interHLY = y;

                x = centerX;
                y = centerY;
                while (y < Ey && x < Bx - Ay + Cy && x + y > Bx - Ay + Cy + Dy)
                {
                    x++;
                    y++;
                }
                interDRX = x;
                interDRY = y;

                Axb = Mathf.Max(Mathf.Min(interHLX-1,Ax),Ax-Hy+Ay);
                Ayb = interHLX>Ax ? Ay : interHLY;
                Bxb = Mathf.Max(interHLX-1,Ax-Hy+Ay);
                Cyb = interDRY;
                Dyb = Mathf.Max(interDRY,Dy);
                Eyb = Ey;
                Gyb = Gy;
                Hyb = Mathf.Max(interHLY, Hy);

                Axt = Mathf.Max(interHLX+1, Ax);
                Ayt = Ay;
                Bxt = Bx;
                Cyt = Cy;
                Dyt = Mathf.Min(interDRY, Dy);
                Eyt = interDRY;
                Gyt = interHLY;
                Hyt = Mathf.Min(interHLY, Hy);

                checkb = CheckRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb);
                checkt = CheckRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt);

                if (checkb)
                {
                    if (checkt)
                    {
                        MakeRoom(Axb, Ayb, Bxb, Cyb, Dyb, Eyb, Gyb, Hyb, numero_room * 2, 0);
                        MakeRoom(Axt, Ayt, Bxt, Cyt, Dyt, Eyt, Gyt, Hyt, numero_room * 2 + 1, 0);
                    }
                    else
                    {
                        MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                    }
                }
                else
                {
                    MakeRoom(Ax, Ay, Bx, Cy, Dy, Ey, Gy, Hy, numero_room, failed_attempt + 1);
                }
            }
            //negative diagonal cut
            else
            {

            }*/
        }


    }

    bool CheckRoom(int Ax, int Ay, int Bx, int Cy, int Dy, int Ey, int Gy, int Hy)
    {
        bool result;
        result = true;
        if (Bx - Ax < 6 || Dy - Cy < 6 || Gy - Hy < 6 || Bx + Cy + Dy + Hy + Gy - 2 * Ay - 2 * Ey < 6)
        {
            result = false;
        }

        return result;
    }


}
