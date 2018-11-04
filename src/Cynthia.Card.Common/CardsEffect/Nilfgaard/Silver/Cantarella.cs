using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("63004")]//坎塔蕾拉
    public class Cantarella : CardEffect
    {
        public Cantarella(IGwentServerGame game, GameCard card) : base(game, card) { }
        public bool IsUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //这张卡只能用一次
            if (IsUse) return 0;
            IsUse = true;
            //选择卡组随机两张卡(另一半场玩家)
            var list = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].Mess().Take(2).ToList();
            //让玩家(另一半场)选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(Game.AnotherPlayer(Card.PlayerIndex), list, isCanOver: false);
            if (result.Count == 0) return 0;
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(row, row.IndexOf(dcard), row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(Game.AnotherPlayer(Card.PlayerIndex));//抽卡
            if (list.Count() > 1)//将另一张卡移动到末尾
                await Game.LogicCardMove(row, row.IndexOf(list.Where(x => x != dcard).Single()), row, row.Count);
            return 0;
        }
    }
}