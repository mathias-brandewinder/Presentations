# m-brace demos

2 demos of the distributed computation and data processing library [m-brace](http://mbrace.io/). Both are intended to be run using the [m-brace starter kit](https://github.com/mbraceproject/MBrace.StarterKit).

## Digits recognizer

This demo illustrates how m-brace can be used for 'big compute'. Starting from a simple local script, digits-local.fsx, running a k-nearest-neighbors classifier on the MINST dataset, we convert the script to digits-cloudified.fsx, to run computations across a cluster. The last example shows how this can be used, for instance, to compute in parallel the accuracy of models using various values of k.

## Guardian wordcount

This demo illustrates how m-brace can be used to process larger datasets, without being limited by the local machine. In guardian.fsx, we retrieve one year of headlines from the Guardian, using the JSON type provider, and persist it on the cluster, without touching the local machine. We then use a C# library (the Analyzer.cs file) to perform a word count, using CloudFlow to process the data. The year-in-words.fsx creates a WPF visualization using that data, highlighting day-by-day 'important' words from the headlines.
