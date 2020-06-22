using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataProgressSprite", menuName = "ScriptableObjects/DataProgressSprite")]
public class DataProgressSprite : ScriptableObject
{
    public enum SpriteNeeded { Unkillable, Immaculate,WellProtected,Sniper,Speedrunner, Unphotogenic ,AllBonus,LivingArmor,Inextremis,Chouchou,TechWizard,WhoNeedsAShotgun,GravityIsWeak,Unshakable, Environmentalist ,Gladiator, GameFinished, Juggernaut, Aikent, Fanfaron }
    public Sprite Unkillable = null;
    public Sprite Immaculate = null;
    public Sprite WellProtected = null;
    public Sprite Sniper = null;
    public Sprite Speedrunner = null;
    public Sprite Unphotogenic = null;
    public Sprite AllBonus = null;
    public Sprite LivingArmor = null;
    public Sprite Inextremis = null;
    public Sprite Chouchou = null;
    public Sprite TechWizard = null;
    public Sprite WhoNeedsAShotgun = null;
    public Sprite GravityIsWeak = null;
    public Sprite Unshakable = null;
    public Sprite Environmentalist = null;
    public Sprite Gladiator = null;
    public Sprite GameFinished = null;
    public Sprite Juggernaut = null;
    public Sprite Aikent = null;
    public Sprite Fanfaron = null;

    public Sprite getSprite(int spriteType)
    {
        if (spriteType <= (int)SpriteNeeded.Fanfaron)
        {
            switch (spriteType)
            {
                case (int)SpriteNeeded.Unkillable:
                    return Unkillable;
                case (int)SpriteNeeded.Immaculate:
                    return Immaculate;
                case (int)SpriteNeeded.WellProtected:
                    return WellProtected;
                case (int)SpriteNeeded.Sniper:
                    return Sniper;
                case (int)SpriteNeeded.Speedrunner:
                    return Speedrunner;
                case (int)SpriteNeeded.Unphotogenic:
                    return Unphotogenic;
                case (int)SpriteNeeded.AllBonus:
                    return AllBonus;
                case (int)SpriteNeeded.LivingArmor:
                    return LivingArmor;
                case (int)SpriteNeeded.Inextremis:
                    return Inextremis;
                case (int)SpriteNeeded.Chouchou:
                    return Chouchou;
                case (int)SpriteNeeded.TechWizard:
                    return TechWizard;
                case (int)SpriteNeeded.WhoNeedsAShotgun:
                    return WhoNeedsAShotgun;
                case (int)SpriteNeeded.GravityIsWeak:
                    return GravityIsWeak;
                case (int)SpriteNeeded.Unshakable:
                    return Unshakable;
                case (int)SpriteNeeded.Environmentalist:
                    return Environmentalist;
                case (int)SpriteNeeded.Gladiator:
                    return Gladiator;
                case (int)SpriteNeeded.GameFinished:
                    return GameFinished;
                case (int)SpriteNeeded.Juggernaut:
                    return Juggernaut;
                case (int)SpriteNeeded.Aikent:
                    return Aikent;
                case (int)SpriteNeeded.Fanfaron:
                    return Fanfaron;
            }
        }
        return null;
    }

}
