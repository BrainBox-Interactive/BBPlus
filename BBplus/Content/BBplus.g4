grammar BBplus;

program: line* EOF;

mod: 'mod' IDENTIFIER '{' line* '}';
class: 'class' IDENTIFIER '{' line* '}';
line: statement | ifBlock | whileBlock | functionBlock | mod | class;
statement: (assignement | functionCall) ';';

ifBlock: IF '(' expression ')' block (ELSE elseIfBlock)?;
elseIfBlock: block | ifBlock;
whileBlock: WHILE '(' expression ')' block (ELSE elseIfBlock)?;
functionBlock: (MOD '.')? IDENTIFIER '=>' (PRIVATE?) FUNCTION '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;

assignement:  IDENTIFIER (PRIVATE?) '=>' expression;
functionCall: (MOD '.')? IDENTIFIER (':' expression (',' expression)*)?;
expression
    : constant                                  #constantExpr
    | IDENTIFIER                                #idExpr
    | functionCall                              #functionCallExpr
    | block                                     #blockExpr
    | '(' expression ')'                        #parenExpr
    | '!' expression                            #notExpr
    | expression mult expression                #multExpr
    | expression add expression                 #addExpr
    | expression cmp expression                 #cmpExpr
    | expression bool expression                #boolExpr
    ;

mult: '*' | '/' | '%';
add: '+' | '-';
cmp: '<' | '>' | '<=' | '>=' | '==' | '!=';
bool: BOOL_OPERATOR;

BOOL_OPERATOR: '&&' | '||' | '|||';

constant: INTEGER | FLOAT | STRING | BOOL | NULL;

INTEGER: [0-9]+;
FLOAT: [0-9]+'.'[0-9]+;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOL: 'true' | 'false';
NULL: 'null' | 'nil';

WHILE: 'while' | 'until';
IF: 'if' | 'when';
ELSE: 'else' | 'or';
FUNCTION: 'fn' | 'function';
PRIVATE: '@';

block: '{' line* '}';

WS: [ \t\r\n]+ -> skip;
SINGLELINE_COMMENT: '//' ~[\r\n]* -> skip;
MULTILINE_COMMENT: '/*' ~'*' ('*' ~'/')* '*'? '/' -> skip;

IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
MOD: [a-zA-Z_][a-zA-Z0-9_]*;
TYPE: 'number' | 'str' | 'bool' | 'void';
