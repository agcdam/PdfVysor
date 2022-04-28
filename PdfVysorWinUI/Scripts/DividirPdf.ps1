param([string]$documento=$(throw "Documento requerido"), #Introducimos el path del documento que queremos dividir
[int]$numeroPaginas=$(throw "Introduce el numero de paginas"), #Introducimos la pagina por la que queremos dividir siendo el 1 la primera
[string]$outPath=$PSScriptRoot, #Introducimos la ruta de salida siendo la default el directorio del usuario
[string]$outName="resultado") #Introducimos el nombre de salida del fichero resultante siendo resultado el default

$tmpName = "ficTemp"

function Delete {
    param ([string]$Path)
    Remove-Item -Path $path
}

# dividimos el fichero por la pagina introducida en la ruta establecida con un nombre temporal
Split-PDF -FilePath $documento -SplitCount $numeroPaginas -OutputFolder $outPath -OutputName $tmpName

# se divide el fichero en un numero x de ficheros siendo x el numero de paginas siendo cada fichero una pagina distinta
#Split-PDF -FilePath $documento -OutputFolder $outPath -OutputName "outFile" 

# comprobacion si existe un fichero pdf con el nombre que queremos usar
if (Test-Path -Path ($outPath + "\" + $outName + ".pdf") -PathType Leaf) {
    Write-Host "existe"
    # si existe un fichero con el nombre de nuestro fichero lo eliminamos
    Delete -Path ($outPath + "\" + $outName + "*.pdf")
    # renombramos el fichero temporal al nombre establecido por el usuario
    # (siendo el fichero 0 el que mantendremos y siendo el resto eliminado si la siguiente linea no es comentada)
    Rename-Item -Path ($outPath + "\" + $tmpName + "0.pdf") -NewName ($outName + ".pdf") -Force
} else {
    Write-Host "no existe"

    # renombramos el fichero temporal al nombre establecido por el usuario
    # (siendo el fichero 0 el que mantendremos y siendo el resto eliminado si la siguiente linea no es comentada)
    Rename-Item -Path ($outPath + "\" + $tmpName + "0.pdf") -NewName ($outName + ".pdf") -Force
}


# eliminamos todos los ficheros que han sido generados anteriormente
# (si se comenta esta linea tendriamos el resto del documento dividido en ficheros de la misma longitud
# ej. fichero de 6 pag. dividido por la pag. 2 siendo como resultado 3 ficheros de 2 paginas cada uno)
Delete -Path ($outPath + "\" + $tmpName + "*.pdf")

