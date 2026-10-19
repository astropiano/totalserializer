namespace Astropiano.TotalSerializer.Test.TestCases.Array
{
    public class ArrayWithDelegateTestClass
    {
        public string[] PropertyMarkerArray { get; set; }

        public delegate void MarkerAction(int index, ArrayWithDelegateTestClass instance);

        public MarkerAction[] DelegateArray { get; set; }

        public void ResetMarkers(int size)
        {
            PropertyMarkerArray = new string[size];
            for (int i = 0; i < PropertyMarkerArray.Length; i++)
            {
                PropertyMarkerArray[i] = "";
            }
        }

        /// <summary>
        /// Create a new object with arrays of length <paramref name="size"/>.
        /// </summary>
        public static ArrayWithDelegateTestClass Create(int size)
        {
            ArrayWithDelegateTestClass o = new ArrayWithDelegateTestClass();
            o.ResetMarkers(size);
            
            void MarkerAction(int index, ArrayWithDelegateTestClass instance)
            {
                instance.PropertyMarkerArray[index] = "marked_" + index;
            }

            o.DelegateArray = new MarkerAction[size];
            for (int i = 0; i < o.DelegateArray.Length; i++)
            {
                o.DelegateArray[i] = MarkerAction;
            }

            return o;
        }
    }
}