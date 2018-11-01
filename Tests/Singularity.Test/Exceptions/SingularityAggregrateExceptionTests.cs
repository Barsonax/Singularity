using System;
using System.Collections.Generic;

using Singularity.Exceptions;

using Xunit;

namespace Singularity.Test.Exceptions
{
    public class SingularityAggregrateExceptionTests
    {
		[Fact]
	    public void FirstLevel_IsCorrect()
	    {
			var exception = new SingularityAggregateException("1", new List<Exception>());

		    string[] lines = exception.Message.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			Assert.Equal("1",lines[0]);
		}

	    [Fact]
	    public void SecondLevel_IsCorrect()
	    {

		    var exception = new SingularityAggregateException("1", new List<Exception>
		    {
			    new SingularityAggregateException("21", new List<Exception>
			    {
				    new Exception("211"),
				    new Exception("212"),
				    new Exception("213"),
			    }),
			    new SingularityAggregateException("22", new List<Exception>
			    {
				    new Exception("221"),
				    new Exception("222"),
				    new Exception("223"),
			    })
		    });

		    string[] lines = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		    Assert.Equal("1", lines[0]);

		    Assert.Equal("	21", lines[1]);
		    Assert.Equal("		211", lines[2]);
		    Assert.Equal("		212", lines[3]);
		    Assert.Equal("		213", lines[4]);

		    Assert.Equal("	22", lines[5]);
		    Assert.Equal("		221", lines[6]);
		    Assert.Equal("		222", lines[7]);
		    Assert.Equal("		223", lines[8]);
	    }

	    [Fact]
	    public void ThirdLevel_IsCorrect()
	    {

		    var exception = new SingularityAggregateException("1", new List<Exception>
		    {
			    new SingularityAggregateException("21", new List<Exception>
			    {
				    new Exception("211"),
				    new Exception("212"),
				    new Exception("213"),
			    }),
			    new SingularityAggregateException("22", new List<Exception>
			    {
				    new Exception("221"),
				    new Exception("222"),
				    new Exception("223"),
			    })
		    });

		    string[] lines = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		    Assert.Equal("1", lines[0]);

		    Assert.Equal("	21", lines[1]);
		    Assert.Equal("		211", lines[2]);
		    Assert.Equal("		212", lines[3]);
		    Assert.Equal("		213", lines[4]);

		    Assert.Equal("	22", lines[5]);
		    Assert.Equal("		221", lines[6]);
		    Assert.Equal("		222", lines[7]);
		    Assert.Equal("		223", lines[8]);
	    }

	    [Fact]
	    public void ThirdLevel_Mixed_IsCorrect()
	    {

		    var exception = new SingularityAggregateException("1", new List<Exception>
		    {
			    new SingularityAggregateException("21", new List<Exception>
			    {
				    new Exception("211"),
				    new Exception("212"),
				    new Exception("213"),
			    }),
			    new SingularityAggregateException("22", new List<Exception>
			    {
				    new Exception("221"),
				    new Exception("222"),
				    new Exception("223"),
			    }),
				new Exception("23"),
			    new Exception("24"),
			    new Exception("25")
			});

		    string[] lines = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		    Assert.Equal("1", lines[0]);

		    Assert.Equal("	21", lines[1]);
		    Assert.Equal("		211", lines[2]);
		    Assert.Equal("		212", lines[3]);
		    Assert.Equal("		213", lines[4]);

		    Assert.Equal("	22", lines[5]);
		    Assert.Equal("		221", lines[6]);
		    Assert.Equal("		222", lines[7]);
		    Assert.Equal("		223", lines[8]);

		    Assert.Equal("	23", lines[9]);
		    Assert.Equal("	24", lines[10]);
		    Assert.Equal("	25", lines[11]);
		}
	}
}
