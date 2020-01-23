﻿public interface IEntity
{
    void TakeDamage(float value);

    void OnAttack(DataUiTemporarySprite dataSpriteShield, DataUiTemporarySprite dataSpriteLife);

    void Heal(float value);
}

