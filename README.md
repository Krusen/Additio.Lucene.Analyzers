# Danish Analyzer for Lucene.NET

I was looking for a danish analyzer for [Lucene.Net](https://github.com/apache/lucenenet) but couldn't find any. 
There exists on in the [Lucene project](http://lucene.apache.org/core/) (JAVA) and it has somewhat recently been ported to [Lucene.Net](https://github.com/apache/lucenenet)
but [Lucene.Net](https://github.com/apache/lucenenet) haven't released a new version since 2012.

This analyzer is basically just composed of the already existing stop words and danish stemmer in [Lucene.Net](https://github.com/apache/lucenenet).

## Sitecore ContentSearch

This was made for use with Sitecore ContentSearch. I've included an [example config file](Sitecore.ContentSearch/z.Additio.Lucene.DefaultIndexConfiguration.DanishExecutionContext.config)
that adds the danish analyzer as a `CultureExecutionContext` for the danish (`da`) culture.

> **NOTE:** Make sure that the config file is sorted after `Sitecore.ContentSearch.Lucene.DefaultIndexConfiguration.config`

```XML
<mapEntry type="Sitecore.ContentSearch.LuceneProvider.Analyzers.PerExecutionContextAnalyzerMapEntry, Sitecore.ContentSearch.LuceneProvider">
  <param hint="executionContext" type="Sitecore.ContentSearch.CultureExecutionContext, Sitecore.ContentSearch">
    <param hint="cultureInfo" type="System.Globalization.CultureInfo, mscorlib">
      <param hint="name">da</param>
    </param>
  </param>
  <param hint="analyzer" type="Sitecore.ContentSearch.LuceneProvider.Analyzers.DefaultPerFieldAnalyzer, Sitecore.ContentSearch.LuceneProvider">
    <param hint="defaultAnalyzer" type="Additio.Lucene.Analyzers.DanishAnalyzer, Additio.Lucene.Analyzers">
      <param hint="version">Lucene_30</param>
    </param>
  </param>
</mapEntry>
```


This means the analyzer will be used when indexing the danish (`da`) language version of items and if you search 
using a danish (`da`) `CultureExecutionContext`.

For example like this:

```csharp
using (var context = ContentSearchManager.GetIndex("INDEX_NAME").CreateSearchContext())
{
    var cultureExecutionContext = new CultureExecutionContext(new CultureInfo("da"));
    var query = context.GetQueryable<SearchResultItem>(cultureExecutionContext);

    // configure query and get results
}
```