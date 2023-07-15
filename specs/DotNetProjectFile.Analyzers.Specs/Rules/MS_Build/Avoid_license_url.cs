﻿namespace Rules.MS_Build.Avoid_license_url;

public class Reports
{
    [Test]
    public void on_license_url()
       => new AvoidLicenseUrl()
       .ForProject("WithLicenseUrl.cs")
       .HasIssue(
           new Issue("Proj0211", "Replace deprecated <PackageLicenseUrl> with <PackageLicenseExpression> or <PackageLicenseFile> node."));
}

public class Guards
{
    [TestCase("CompliantCSharp.cs")]
    [TestCase("CompliantCSharpPackage.cs")]
    [TestCase("CompliantVB.vb")]
    [TestCase("CompliantVBPackage.vb")]
    public void Projects_without_issues(string project)
         => new AvoidLicenseUrl()
        .ForProject(project)
        .HasNoIssues();
}
