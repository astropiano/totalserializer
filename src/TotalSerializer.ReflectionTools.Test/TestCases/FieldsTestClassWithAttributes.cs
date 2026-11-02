namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    public class FieldsTestClassWithAttributes
    {
        public bool PostWriteLogicCalled;
        
        [PostWriteAction(nameof(PostWriteLogic))]
        public int Integer;

        public void PostWriteLogic()
        {
            PostWriteLogicCalled = true;
        }
    }
}
