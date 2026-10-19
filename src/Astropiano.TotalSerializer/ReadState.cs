namespace Astropiano.TotalSerializer {
    public enum ReadState
    {
        OutsideObj,
        ReadingObj,
        ReadingTypeInfo,
        ReadingIdInfo,
        ReadingProperty,
        ReadingPseudoProperty
    }
}