using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
using Sitecore.Data.Comparers;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
using Sitecore.XA.Foundation.Variants.Abstractions;
using Sitecore.XA.Foundation.Variants.Abstractions.Pipelines.GetVariants;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sitecore.Support.XA.Foundation.Variants.Abstractions.Pipelines.GetVariants
{
  public class FilterVariants : GetVariantsBase
  {
    public FilterVariants(IContentRepository contentRepository)
    {
      this.ContentRepository = contentRepository;
    }

    protected virtual bool AllowedInTemplate(Item item, string pageTemplateId)
    {
      Field field = item.Fields[Templates.IVariantDefinition.Fields.AllowedInTemplates];
      if ((field != null) && ((field.Value != string.Empty) || !this.InheritsFromAllowedTemplate(pageTemplateId)))
      {
        return field.Value.Contains(pageTemplateId);
      }
      return true;
    }

    protected virtual bool InheritsFromAllowedTemplate(string pageTemplateId)
    {
      TemplateItem templateItem = this.ContentRepository.GetTemplate(new ID(pageTemplateId));
      return Sitecore.XA.Foundation.Variants.Abstractions.Configuration.AllowedTemplates.Any<ID>(id => templateItem.DoesTemplateInheritFrom(id));
    }

    public void Process(GetVariantsArgs args)
    {
      args.Variants = (from i in args.Variants
                       where this.AllowedInTemplate(i, args.PageTemplateId)
                       select i).Distinct<Item>(new ItemIdComparer()).ToList<Item>();
    }

    public IContentRepository ContentRepository { get; set; }
  }
}
