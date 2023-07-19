#nullable enable
namespace AST.Hlsl {
    public interface Trivia {
        public interface PreprocessorDirective : Trivia {
            public record Define : PreprocessorDirective {
                public Identifier id     { get; set; }
                public string[]   tokens { get; set; }
            }
            
            public record Undef : PreprocessorDirective {
                public Identifier id { get; set; }
            }

            public record Ifdef : PreprocessorDirective {
                public Identifier id { get; set; }
            }
            
            public record Ifndef : PreprocessorDirective {
                public Identifier id { get; set; }
            }
            
            public record If : PreprocessorDirective {
                public string[] tokens { get; set; }
            }
            
            public record Elif : PreprocessorDirective {
                public Identifier id { get; set; }
            }
            
            public record Endif : PreprocessorDirective { }
            
            public record Else : PreprocessorDirective { }
            
            public record Error : PreprocessorDirective {
                public string[] tokens { get; set; }
            }

            public record Include : PreprocessorDirective {
                public string path { get; set; }
            }
            
            public record Line : PreprocessorDirective {
                public uint   line { get; set; }
                public string file { get; set; }
            }
            
            public record Pragma : PreprocessorDirective {
                public string[] tokens { get; set; }
            }
        }

        public interface Whitespace : Trivia {
            public string? text { get; set; }
        }

        public interface Comment : Trivia {
            public string? text { get; set; }
            
            public record Line : Comment {
                public string? text { get; set; }
            }
            public record Block : Comment {
                public string? text { get; set; }
            }
        }
    }
}
