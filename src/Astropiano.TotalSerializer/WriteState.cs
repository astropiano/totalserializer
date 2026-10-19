namespace Astropiano.TotalSerializer {
    public enum WriteState
    {
        OutsideObj,
        WritingObject,
        WritingTypeInfo,
        WritingPropertyName,
        WritingPropertyContent,
        WritingArray
    }
}