namespace Fody.Constructors.Internal {
   public class FieldInfo {
      /// <summary>
      /// Whether the field has the readonly modifier
      /// </summary>
      public bool IsInitOnly { get; set; }

      /// <summary>
      /// Whether the field has an initializer and the initializer
      /// sets the field to a non-default value (not default(T)).
      /// </summary>
      public bool HasNonDefaultFieldInitializer { get; set; }
   }
}
