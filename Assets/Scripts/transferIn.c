int wordCount = 0;
int lineCount = 0;
//while loop shit


int countWordsInLine(char * line) {
    int wordCount = 0;
    char * strptr;
    strptr = strtok(line, " ");
    while (strptr != NULL) {
        strptr = strtok(NULL, line);
        wordCount++;
    }
    return wordCount;
}