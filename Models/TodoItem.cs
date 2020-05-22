using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
  public class TodoItem
  {
    //set attribute
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Progress { get; set; }

    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; }
  }
}