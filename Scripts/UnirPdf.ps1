
#Pasamos por los parametros de cmd todos los ficheros que queremos unir
#Se uniran uno detras de otro
Param([String[]] $files)

Merge-PDF -InputFile $files -OutputFile ($PSScriptRoot + "\out.pdf")