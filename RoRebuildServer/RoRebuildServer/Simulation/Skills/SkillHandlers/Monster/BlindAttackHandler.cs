﻿using RebuildSharedData.Data;
using RebuildSharedData.Enum;
using RebuildSharedData.Enum.EntityStats;
using RoRebuildServer.EntityComponents;
using RoRebuildServer.Networking;

namespace RoRebuildServer.Simulation.Skills.SkillHandlers.Monster
{
    [SkillHandler(CharacterSkill.Blind, SkillClass.Physical)]
    public class BlindAttackHandler : SkillHandlerBase
    {
        public override int GetSkillRange(CombatEntity source, int lvl) => 7;
        public override void Process(CombatEntity source, CombatEntity? target, Position position, int lvl, bool isIndirect)
        {
            if (target == null)
                return;

            var res = source.CalculateCombatResult(target, 1f, 1, AttackFlags.Physical, CharacterSkill.Blind, AttackElement.None);
            source.ApplyCooldownForAttackAction(target);
            source.ExecuteCombatResult(res, false);

            var ch = source.Character;

            CommandBuilder.SkillExecuteTargetedSkillAutoVis(source.Character, target.Character, CharacterSkill.Blind, lvl, res);

            if (!res.IsDamageResult)
                return;

            source.TryBlindTarget(target, lvl * 200, res.AttackMotionTime + 0.3f);
        }
    }
}
