using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Encyclopedia.Pages;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Extensions;

namespace BetterHeroEncyclopedia
{
    class Main : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            // create a harmony patcher
            Harmony harmony = new Harmony("org.khallshock.betterheroencyclopedia");
            harmony.PatchAll();
        }

        public class EncyclopediaListHeroSkillComparer : DefaultEncyclopediaHeroPage.EncyclopediaListHeroComparer
        {
            // the skill used to create the comparer
            SkillObject _skill;

            public EncyclopediaListHeroSkillComparer(SkillObject skill)
            {
                _skill = skill;
            }

            public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
            {
                // cast the list items to heros
                Hero hero1 = (Hero)x.Object;
                Hero hero2 = (Hero)y.Object;
                // if skills are equal
                if (hero1.GetSkillValue(_skill) == hero2.GetSkillValue(_skill))
                {
                    return 0;
                }
                if (IsAscending)
                {
                    // return the greater skill first
                    return (hero1.GetSkillValue(_skill) > hero2.GetSkillValue(_skill)) ? 1 : -1;
                }
                // return the greater skill last
                return (hero1.GetSkillValue(_skill) > hero2.GetSkillValue(_skill)) ? -1 : 1;
            }

            public override string GetComparedValueText(EncyclopediaListItem item)
            {
                // return the skill level as string
                return ((Hero)item.Object).GetSkillValue(_skill).ToString();
            }
        }

        // the harmony patcher
        [HarmonyPatch(typeof(DefaultEncyclopediaHeroPage), "InitializeSortControllers")]
        class Patch
        {
            static void Postfix(ref IEnumerable<EncyclopediaSortController> __result)
            {
                // loop through all skills in the game
                foreach (SkillObject skill in Skills.All)
                {
                    // create a sorter for each one of them
                    ((List<EncyclopediaSortController>)__result).Add(new EncyclopediaSortController(new TextObject(skill.Name.ToString(), null), new EncyclopediaListHeroSkillComparer(skill)));
                }
            }
        }
    }
}

