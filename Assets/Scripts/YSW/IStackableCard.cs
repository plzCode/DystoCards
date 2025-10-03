namespace DystoCards.Cards.Interfaces
{
    public interface IStackableCard
    {
        bool CanStack(Card other);
        void Stack(Card other);
    }
}